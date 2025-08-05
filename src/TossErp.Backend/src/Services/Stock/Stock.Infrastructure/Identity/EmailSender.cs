using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace TossErp.Stock.Infrastructure.Identity;

public class EmailSender : IEmailSender<ApplicationUser>
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        var subject = "Confirm your email address";
        var body = $@"
            <html>
            <body>
                <h2>Email Confirmation</h2>
                <p>Hello {user.UserName},</p>
                <p>Please confirm your email address by clicking the link below:</p>
                <p><a href='{confirmationLink}'>Confirm Email</a></p>
                <p>If you didn't create an account, please ignore this email.</p>
                <p>Best regards,<br/>TOSS ERP Team</p>
            </body>
            </html>";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        var subject = "Password Reset Code";
        var body = $@"
            <html>
            <body>
                <h2>Password Reset</h2>
                <p>Hello {user.UserName},</p>
                <p>You requested a password reset. Your reset code is:</p>
                <p><strong>{resetCode}</strong></p>
                <p>This code will expire in 15 minutes.</p>
                <p>If you didn't request a password reset, please ignore this email.</p>
                <p>Best regards,<br/>TOSS ERP Team</p>
            </body>
            </html>";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        var subject = "Reset Your Password";
        var body = $@"
            <html>
            <body>
                <h2>Password Reset</h2>
                <p>Hello {user.UserName},</p>
                <p>You requested a password reset. Click the link below to reset your password:</p>
                <p><a href='{resetLink}'>Reset Password</a></p>
                <p>This link will expire in 1 hour.</p>
                <p>If you didn't request a password reset, please ignore this email.</p>
                <p>Best regards,<br/>TOSS ERP Team</p>
            </body>
            </html>";

        await SendEmailAsync(email, subject, body);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            var smtpSettings = _configuration.GetSection("EmailSettings");
            var smtpHost = smtpSettings["SmtpHost"];
            var smtpPort = int.Parse(smtpSettings["SmtpPort"] ?? "587");
            var smtpUsername = smtpSettings["SmtpUsername"];
            var smtpPassword = smtpSettings["SmtpPassword"];
            var fromEmail = smtpSettings["FromEmail"];
            var fromName = smtpSettings["FromName"] ?? "TOSS ERP";
            var enableSsl = bool.Parse(smtpSettings["EnableSsl"] ?? "true");

            // If SMTP is not configured, fall back to console logging
            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUsername))
            {
                _logger.LogInformation("SMTP not configured. Email would be sent to {Email} with subject: {Subject}", toEmail, subject);
                Console.WriteLine($"Email to {toEmail}: {subject}");
                Console.WriteLine($"Body: {body}");
                return;
            }

            using var smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = enableSsl
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail ?? "noreply@toss-erp.com", fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent successfully to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            // For development, also log to console
            Console.WriteLine($"Email sending failed to {toEmail}: {ex.Message}");
            throw;
        }
    }
} 
