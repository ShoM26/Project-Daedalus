namespace ProjectDaedalus.Core.Interfaces
{
    /// <summary>
    /// Service for sending emails
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email asynchronously
        /// </summary>
        /// <param name="toEmail">Recipient email address</param>
        /// <param name="subject">Email subject line</param>
        /// <param name="htmlBody">HTML content of the email</param>
        /// <returns>Task representing the async operation</returns>
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
    }
}