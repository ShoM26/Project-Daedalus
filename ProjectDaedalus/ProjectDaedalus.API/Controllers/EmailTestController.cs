using Microsoft.AspNetCore.Mvc;
using ProjectDaedalus.Core.Interfaces;

namespace ProjectDaedalus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailTestController : ControllerBase
    {
        private readonly IEmailTemplateService _templateService;
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailTestController> _logger;

        public EmailTestController(
            IEmailTemplateService templateService,
            IEmailService emailService,
            ILogger<EmailTestController> logger)
        {
            _templateService = templateService;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost("send-test")]
        public async Task<IActionResult> SendTestEmail([FromBody] TestEmailRequest request)
        {
            try
            {
                // Render the template
                var html = _templateService.RenderTemplate("LowMoistureAlert", new Dictionary<string, string>
                {
                    { "PlantName", request.PlantName ?? "Test Fern" },
                    { "CurrentMoisture", request.CurrentMoisture ?? "15" },
                    { "ThresholdValue", request.ThresholdValue ?? "30" },
                    { "DeviceName", request.DeviceName ?? "PLANT_TEST_12345" },
                    { "DashboardUrl", "https://localhost:7001/plants/1" },
                    { "Timestamp", DateTime.Now.ToString("MMMM d, yyyy 'at' h:mm tt") }
                });

                // Send the email
                await _emailService.SendEmailAsync(
                    request.ToEmail, 
                    "🌱 Low Moisture Alert - Test Email", 
                    html);

                _logger.LogInformation("Test email sent to {Email}", request.ToEmail);

                return Ok(new { 
                    message = "Test email sent successfully!", 
                    sentTo = request.ToEmail 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send test email");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    public class TestEmailRequest
    {
        public string ToEmail { get; set; }
        public string? PlantName { get; set; }
        public string? CurrentMoisture { get; set; }
        public string? ThresholdValue { get; set; }
        public string? DeviceName { get; set; }
    }
}