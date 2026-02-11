using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectDaedalus.Core.Configurations;
using ProjectDaedalus.Core.Entities;
using ProjectDaedalus.Core.Interfaces;
using ProjectDaedalus.Infrastructure.Data;

namespace ProjectDaedalus.Infrastructure.Services
{
    public class NotificationWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationWorker> _logger;
        private readonly NotificationWorkerSettings _settings;

        public NotificationWorker(
            IServiceProvider serviceProvider,
            ILogger<NotificationWorker> logger,
            IOptions<NotificationWorkerSettings> settings)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _settings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_settings.Enabled)
            {
                _logger.LogInformation("NotificationWorker is disabled in configuration");
                return;
            }

            _logger.LogInformation(
                "NotificationWorker started - running every {Interval} minutes", 
                _settings.IntervalMinutes);

            // Wait a bit before first run (let the app finish starting up)
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessNotifications(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in NotificationWorker processing loop");
                }

                // Wait for next interval
                await Task.Delay(
                    TimeSpan.FromMinutes(_settings.IntervalMinutes), 
                    stoppingToken);
            }

            _logger.LogInformation("NotificationWorker stopped");
        }

        private async Task ProcessNotifications(CancellationToken cancellationToken)
        {
            _logger.LogInformation("NotificationWorker: Starting notification check");

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DaedalusContext>();
            var templateService = scope.ServiceProvider.GetRequiredService<IEmailTemplateService>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var today = DateTime.UtcNow.Date;

            // The big query: Find all plants that need alerts
            var plantsNeedingAlerts = await context.UserPlants
                .Include(up => up.Plant)
                .Include(up => up.User)
                .Include(up => up.Device)
                .Where(up =>
                    // Has a recent sensor reading that's below threshold
                    context.SensorHistories
                        .Where(sr => sr.DeviceId == up.DeviceId)
                        .OrderByDescending(sr => sr.TimeStamp)
                        .Take(1)
                        .Any(sr => sr.MoistureLevel < up.Plant.MoistureLowRange)
                    // AND no notification sent today
                    && !context.NotificationHistory
                        .Any(nh => nh.UserPlantId == up.UserPlantId
                                && nh.NotificationType == NotificationType.LowMoisture
                                && nh.CreatedAt.Date == today)
                )
                .ToListAsync(cancellationToken);

            _logger.LogInformation(
                "Found {Count} plants needing low moisture alerts", 
                plantsNeedingAlerts.Count);

            foreach (var userPlant in plantsNeedingAlerts)
            {
                try
                {
                    await ProcessSinglePlantAlert(
                        userPlant, 
                        context, 
                        templateService, 
                        emailService, 
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex, 
                        "Failed to process alert for UserPlant {UserPlantId}", 
                        userPlant.UserPlantId);
                    // Continue with other plants even if one fails
                }
            }

            _logger.LogInformation("NotificationWorker: Completed notification check");
        }

        private async Task ProcessSinglePlantAlert(
            UserPlant userPlant,
            DaedalusContext context,
            IEmailTemplateService templateService,
            IEmailService emailService,
            CancellationToken cancellationToken)
        {
            // Get the latest sensor reading
            var latestReading = await context.SensorHistories
                .Where(sr => sr.DeviceId == userPlant.DeviceId)
                .OrderByDescending(sr => sr.TimeStamp)
                .FirstOrDefaultAsync(cancellationToken);

            if (latestReading == null)
            {
                _logger.LogWarning(
                    "No sensor readings found for UserPlant {UserPlantId}", 
                    userPlant.UserPlantId);
                return;
            }

            // Create NotificationHistory record (for deduplication)
            var history = new NotificationHistory
            {
                UserPlantId = userPlant.UserPlantId,
                NotificationType = NotificationType.LowMoisture,
                MoistureValue = latestReading.MoistureLevel,
                ThresholdValue = userPlant.Plant.MoistureLowRange,
                CreatedAt = DateTime.UtcNow
            };

            context.NotificationHistory.Add(history);
            await context.SaveChangesAsync(cancellationToken);

            // Create user-facing Notification record
            var message = $"Your {userPlant.Plant.FamiliarName} needs water! " +
                         $"Current moisture: {latestReading.MoistureLevel:F1}% " +
                         $"(threshold: {userPlant.Plant.MoistureLowRange:F1}%)";

            var notification = new Notification
            {
                UserPlantId = userPlant.UserPlantId,
                Message = message,
                NotificationType = NotificationType.LowMoisture,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            context.Notifications.Add(notification);
            await context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Created notification for UserPlant {UserPlantId} - {PlantName}", 
                userPlant.UserPlantId, 
                userPlant.Plant.FamiliarName);

            // Send email (fire and forget, don't let email failures stop processing)
            _ = Task.Run(async () =>
            {
                try
                {
                    var emailHtml = templateService.RenderTemplate("LowMoistureAlert", new Dictionary<string, string>
                    {
                        { "PlantName", userPlant.Plant.FamiliarName },
                        { "CurrentMoisture", latestReading.MoistureLevel.ToString("F1") },
                        { "ThresholdValue", userPlant.Plant.FamiliarName },
                        { "DeviceName", userPlant.Device.HardwareIdentifier },
                        { "DashboardUrl", $"{_settings.DashboardBaseUrl}/plants/{userPlant.UserPlantId}" },
                        { "Timestamp", DateTime.Now.ToString("MMMM d, yyyy 'at' h:mm tt") }
                    });

                    await emailService.SendEmailAsync(
                        userPlant.User.Email,
                        "🌱 Low Moisture Alert",
                        emailHtml);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex, 
                        "Failed to send email for UserPlant {UserPlantId}", 
                        userPlant.UserPlantId);
                }
            }, cancellationToken);
        }
    }
}