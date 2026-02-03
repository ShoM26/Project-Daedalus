using Microsoft.AspNetCore.Mvc;
using ProjectDaedalus.API.Dtos.Notification;
using ProjectDaedalus.Core.Interfaces;

namespace ProjectDaedalus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class NotificationsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<NotificationsController> _logger;
        private readonly INotificationRepository _notificationRepository;

        public NotificationsController(
            IUnitOfWork unitOfWork,
            ILogger<NotificationsController> logger,
            INotificationRepository notificationRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _notificationRepository = notificationRepository;
        }
        
        // GET unread count given userId
        [HttpGet("{userId}/unread-count")]
        public async Task<IActionResult> GetUnreadCount(int userId)
        {
            try
            {
                var unreadCount = await _notificationRepository.GetNotificationsCountAsync(userId);
                return Ok(new {unreadCount});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get unread notification count for user");
                return StatusCode(500, new { error = "Failed to retrieve notification count" });
            }
        }
        // GET list of notifications based on userid
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetNotifications(int userId)
        {
            try
            {
                var query = await _notificationRepository.GetNotificationsByUserIdAsync(userId);

                if (!query.Any())
                {
                    return Ok(new List<NotificationResponseDto>());
                }

                var notifications = query
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(100)
                    .Select(n => new NotificationResponseDto{
                        NotificationId = n.NotificationId,
                        Message = n.Message,
                        NotificationType = n.NotificationType.ToString(),
                        IsRead = n.IsRead,
                        CreatedAt = n.CreatedAt,
                        UserPlantId = n.UserPlantId,
                        UserPlantName = n.UserPlant.Plant.FamiliarName}).ToList();

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get notifications for user");
                return StatusCode(500, new { error = "Failed to retrieve notifications" });
            }
        }

        // PATCH mark singular notification as read
        [HttpPatch("{userId}/{notificationId}/read")]
        public async Task<IActionResult> MarkAsRead(int notificationId, int userId)
        {
            try
            {
                var notification = await _notificationRepository.GetByIdAsync(notificationId);

                if (notification == null)
                {
                    return NotFound(new { error = "Notification not found" });
                }

                if (notification.IsRead)
                {
                    return Ok(new { message = "Notification already marked as read" });
                }

                notification.IsRead = true;
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    "Marked notification {NotificationId} as read for user {UserId}", 
                    notificationId, 
                    userId);

                return Ok(new { success = true, message = "Notification marked as read" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark notification {NotificationId} as read", notificationId);
                return StatusCode(500, new { error = "Failed to update notification" });
            }
        }

        // PATCH Mark all notifications as read
        [HttpPatch("{userId}/mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead(int userId)
        {
            try
            {
                var unreadNotifications = await _notificationRepository.GetUnreadNotificationsByUserIdAsync(userId);
                var count =  unreadNotifications.Count();
                foreach (var notification in unreadNotifications)
                {
                    notification.IsRead = true;
                }

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    "Marked {Count} notifications as read for user {UserId}", 
                    count, 
                    userId);

                return Ok(new { 
                    success = true, 
                    count = count,
                    message = $"Marked {count} notifications as read" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark all notifications as read for user");
                return StatusCode(500, new { error = "Failed to update notifications" });
            }
        }
    }
}