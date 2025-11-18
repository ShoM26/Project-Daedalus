using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectDaedalus.Core.Configuration;
using ProjectDaedalus.Core.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ProjectDaedalus.Infrastructure.Services
{
    public class SendGridEmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<SendGridEmailService> _logger;
        private readonly SendGridClient _client;

        public SendGridEmailService(
            IOptions<EmailSettings> emailSettings,
            ILogger<SendGridEmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
            _client = new SendGridClient(_emailSettings.SendGridApiKey);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                var from = new EmailAddress(_emailSettings.FromEmail, _emailSettings.FromName);
                var to = new EmailAddress(toEmail);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlBody);

                var response = await _client.SendEmailAsync(msg);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation(
                        "Email sent successfully to {Email} with subject '{Subject}'", 
                        toEmail, 
                        subject);
                }
                else
                {
                    var body = await response.Body.ReadAsStringAsync();
                    _logger.LogError(
                        "SendGrid failed with status {StatusCode}: {Body}", 
                        response.StatusCode, 
                        body);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, 
                    "Failed to send email to {Email} with subject '{Subject}'", 
                    toEmail, 
                    subject);
                
                // Don't rethrow - we don't want email failures to crash the worker
                // The notification will still be created in the database
            }
        }
    }
}