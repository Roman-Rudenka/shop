using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Shop.Core.Domain.Interfaces;

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

            _smtpServer = emailSettings["SmtpServer"] ?? throw new ArgumentNullException(nameof(_smtpServer), "Ошибка конфигурации");
            _port = int.TryParse(emailSettings["Port"], out var port) ? port : throw new ArgumentException("Неправильный порт");
            _username = emailSettings["Username"] ?? throw new ArgumentNullException(nameof(_username), "Почта не подтверждена");
            _password = emailSettings["Password"] ?? throw new ArgumentNullException(nameof(_password), "Пароль не подтвержден");
            _fromName = emailSettings["FromName"] ?? "Your Application";
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrEmpty(toEmail)) throw new ArgumentNullException(nameof(toEmail), "Почта не может быть пустой");
            if (string.IsNullOrEmpty(subject)) throw new ArgumentNullException(nameof(subject), "Сообщение не может быть пустым");
            if (string.IsNullOrEmpty(body)) throw new ArgumentNullException(nameof(body), "Сообщение не может быть пустым");

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
                Console.WriteLine($"Сообщение отправлено на {toEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не получилось отправить сообщение на почту {toEmail}. Error: {ex.Message}");
                throw;
            }
        }
    }
}
