using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Identity.API.Infrastructure.Services
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string toEmail, string otpCode);
        Task SendNotificationEmailAsync(string toEmail, string subject, string body);
    }

    public class MailKitEmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<MailKitEmailService> _logger;

        public MailKitEmailService(IConfiguration config, ILogger<MailKitEmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendOtpEmailAsync(string toEmail, string otpCode)
        {
            var subject = "Your Verification OTP - Insurance Policy & Support System";
            var body = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px;'>
                    <h2 style='color: #0056b3;'>Insurance Portal Authentication</h2>
                    <p>Hello,</p>
                    <p>Your One-Time Password (OTP) for account verification is:</p>
                    <div style='background: #f4f4f4; padding: 15px; font-size: 24px; font-weight: bold; letter-spacing: 5px; text-align: center; color: #333;'>
                        {otpCode}
                    </div>
                    <p>This OTP is valid for 10 minutes. Please do not share it with anyone.</p>
                </div>";

            await SendEmailInternalAsync(toEmail, subject, body);
        }

        public async Task SendNotificationEmailAsync(string toEmail, string subject, string body)
        {
            await SendEmailInternalAsync(toEmail, subject, body);
        }

        private async Task SendEmailInternalAsync(string toEmail, string subject, string htmlContent)
        {
            var smtpHost = _config["Smtp:Host"] ?? "smtp.example.com";
            var smtpPort = int.TryParse(_config["Smtp:Port"], out int p) ? p : 587;
            var smtpUser = _config["Smtp:User"] ?? "";
            var smtpPass = _config["Smtp:Password"] ?? "";
            var fromEmail = _config["Smtp:From"] ?? "noreply@insurance-system.com";

            // Always log the email attempt & contents for debugging / offline verification
            _logger.LogInformation("==================================================");
            _logger.LogInformation($"[MAILKIT OTP SERVICE] Target: {toEmail}");
            _logger.LogInformation($"[MAILKIT OTP SERVICE] Subject: {subject}");
            _logger.LogInformation($"[MAILKIT OTP SERVICE] Body preview:\n{htmlContent}");
            _logger.LogInformation("==================================================");

            if (string.IsNullOrEmpty(smtpUser) || smtpHost == "smtp.example.com")
            {
                _logger.LogWarning("[MAILKIT] Standard SMTP credentials not configured. OTP logged to console above.");
                return;
            }

            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(fromEmail));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;

                var builder = new BodyBuilder { HtmlBody = htmlContent };
                email.Body = builder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpUser, smtpPass);
                await client.SendAsync(email);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email via MailKit to {toEmail}");
            }
        }
    }
}
