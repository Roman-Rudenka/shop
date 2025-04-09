using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Shop.Core.Interfaces;
using Shop.Core.Services;

namespace Shop.Infrastructure.Services
{
    public class EmailService : IEmailRepository
    {
        private readonly string _smtpServer;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;
        private readonly string _fromName;

        public EmailService(IConfiguration configuration)
        {
            var emailSettings = configuration.GetSection("EmailSettings");

            _smtpServer = emailSettings["SmtpServer"] ?? throw new ArgumentNullException(nameof(_smtpServer), "SMTP server is not configured");
            _port = int.TryParse(emailSettings["Port"], out var port) ? port : throw new ArgumentException("Invalid port configuration");
            _username = emailSettings["Username"] ?? throw new ArgumentNullException(nameof(_username), "Email username is not configured");
            _password = emailSettings["Password"] ?? throw new ArgumentNullException(nameof(_password), "Email password is not configured");
            _fromName = emailSettings["FromName"] ?? "Your Application";
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrEmpty(toEmail)) throw new ArgumentNullException(nameof(toEmail), "Recipient email address cannot be null or empty");
            if (string.IsNullOrEmpty(subject)) throw new ArgumentNullException(nameof(subject), "Email subject cannot be null or empty");
            if (string.IsNullOrEmpty(body)) throw new ArgumentNullException(nameof(body), "Email body cannot be null or empty");

            using var smtpClient = new SmtpClient(_smtpServer, _port)
            {
                Credentials = new NetworkCredential(_username, _password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_username, _fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"Email sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email to {toEmail}. Error: {ex.Message}");
                throw;
            }
        }
    }
}
