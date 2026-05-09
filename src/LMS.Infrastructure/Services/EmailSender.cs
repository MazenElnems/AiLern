using LMS.Infrastructure.Settings;
using LMS.Application.Contracts.Services;
using LMS.Domain.Models.Notification;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace LMS.Infrastructure.Services;

internal class EmailSender : IEmailSender
{
    private readonly FrontEndSettings _frontEndSettings;
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailSender> _logger;
    private readonly IWebHostEnvironment _env;

    public EmailSender(IOptions<FrontEndSettings> frontEndSettings, ILogger<EmailSender> logger, IWebHostEnvironment env, IOptions<EmailSettings> emailSettings)
    {
        _frontEndSettings = frontEndSettings.Value;
        _logger = logger;
        _env = env;
        _emailSettings = emailSettings.Value;
    }

    public async Task SendChangeEmailConfirmationAsync(int userId, string newEmail, string fullName, string token)
    {
        var templatePath = Path.Combine(_env.WebRootPath, "EmailTemplates", "ChangeEmailConfirmation.html");
        var template = await File.ReadAllTextAsync(templatePath);

        var body = template
            .Replace("{{ConfirmationLink}}", $"http://localhost:5173/confirm-change-email?userId={userId}&newEmail={newEmail}&token={token}")
            .Replace("{{FullName}}", fullName)
            .Replace("{{NewEmail}}", newEmail)
            .Replace("{{ExpiryTime}}", DateTime.UtcNow.AddMinutes(30).ToString("f"));

        await SendAsync(newEmail, fullName, "Change Email Confirmation", body);
    }

    public async Task SendConfirmationEmailAsync(string email, string fullName, string token)
    {
        var templatePath = Path.Combine(_env.WebRootPath, "EmailTemplates", "ConfirmationEmail.html");
        var template = await File.ReadAllTextAsync(templatePath);

        var body = template
            .Replace("{{ConfirmationLink}}", $"{_frontEndSettings.Domain}confirm-email?email={email}&token={token}")
            .Replace("{{FullName}}", fullName);

        await SendAsync(email, fullName, "Email Confirmation", body);
    }

    public async Task SendForgetPasswordEmailAsync(string email, string fullName, string token)
    {
        var templatePath = Path.Combine(_env.WebRootPath, "EmailTemplates", "ResetPassword.html");
        var template = await File.ReadAllTextAsync(templatePath);

        var body = template
            .Replace("{{ResetLink}}", $"http://localhost:5173/set-password?email={email}&token={token}")
            .Replace("{{FullName}}", fullName);

        await SendAsync(email, fullName, "Forget Password", body);
    }

    public async Task SendNotificationEmailWithUrlAsync(string email,string fullName, NotificationEmailModel notificationEmailModel)
    {
        var templatePath = Path.Combine(_env.WebRootPath, "EmailTemplates", "NotificationEmail.html");
        var template = await File.ReadAllTextAsync(templatePath);


        var actionButton = "";

        if (!string.IsNullOrEmpty(notificationEmailModel.ActionUrl))
        {
            actionButton = $@"
                <tr>
                    <td style='padding-top:30px;'>
                        <a href='{notificationEmailModel.ActionUrl}'
                           style='background:#3b82f6;
                                  color:white;
                                  padding:14px 24px;
                                  text-decoration:none;
                                  border-radius:8px;
                                  font-weight:bold;
                                  display:inline-block;'>
                            {notificationEmailModel.ActionText ?? "View Details"}
                        </a>
                    </td>
                </tr>";
        }

        var body = template.Replace("{{FullName}}", fullName)
                           .Replace("{{Title}}", notificationEmailModel.Title)
                           .Replace("{{Message}}", notificationEmailModel.Message)
                           .Replace("{{ActionButton}}", actionButton);

        await SendAsync(email, fullName, "Notification", body);

    }

    public async Task SendWelcomeEmailAsync(string email, string fullName)
    {
        var templatePath = Path.Combine(_env.WebRootPath, "EmailTemplates", "Welcome.html");
        var template = await File.ReadAllTextAsync(templatePath);

        var body = template
            .Replace("{{FullName}}", fullName)
            .Replace("{{DashboardLink}}", _frontEndSettings.Domain);

        await SendAsync(email, fullName, "Welcome", body);
    }

    private async Task SendAsync(string to, string toName ,string subject, string body)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
        message.To.Add(new MailboxAddress(toName, to));
        message.Subject = subject;
        message.Body = new TextPart("html")
        {
            Text = body
        };

        using var smtpClient = new SmtpClient();

        try
        {
            // Connect to SMTP Server
            await smtpClient.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);

            // Authentication
            await smtpClient.AuthenticateAsync(_emailSettings.FromEmail, _emailSettings.AppPassword);

            // Send Message
            await smtpClient.SendAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "an error occurs while sending email to {Email}", to);
        }
        finally
        {
            // Disconnect
            await smtpClient.DisconnectAsync(true);
        }
    }
}
