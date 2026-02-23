using LMS.Application.ConfigurationOptions;
using LMS.Domain.Repositories;
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
            .Replace("{{ResetLink}}", $"{_frontEndSettings.Domain}reset-password?email={email}&token={token}")
            .Replace("{{FullName}}", fullName);

        await SendAsync(email, fullName, "Email Confirmation", body);
    }

    public Task SendWelcomeEmailAsync(string email, string fullName)
    {
        throw new NotImplementedException();
    }

    private async Task SendAsync(string to, string toName ,string subject, string body)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
        message.To.Add(new MailboxAddress(toName, to));
        message.Subject = "Email Confirmation";
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
