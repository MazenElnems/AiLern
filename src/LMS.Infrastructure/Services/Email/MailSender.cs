using LMS.Core.ConfigurationOptions;
using LMS.Domain.Repositories;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace LMS.Infrastructure.Services.Email;

internal class MailSender : IMailSender
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<MailSender> _logger;

    public MailSender(IOptions<EmailSettings> emailSettings, ILogger<MailSender> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
        message.To.Add(new MailboxAddress("Student", to)); 
        message.Subject = subject;
        message.Body = new TextPart("html")
        {
            Text = body
        };

        using var smtpClient = new SmtpClient();

        _logger.LogInformation("sending email to {Email}", to);

        try
        {
            // Connect to SMTP Server
            await smtpClient.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);

            // Authentication
            await smtpClient.AuthenticateAsync(_emailSettings.FromEmail, _emailSettings.AppPassword);

            // Send Message
            await smtpClient.SendAsync(message);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "an error occures while sending email to {Email}", to);
            throw;
        }
        finally
        {
            // Disconnect
            await smtpClient.DisconnectAsync(true);
        }
    }
}
