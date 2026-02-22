using LMS.Application.ConfigurationOptions;
using LMS.Application.Common.Results;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LMS.Domain.Entities.Users;
using LMS.Domain.Errors;

namespace LMS.Application.Features.Auth.Commands.PasswordResetEmail;

public class SendPasswordResetEmailCommandHandler : IRequestHandler<SendPasswordResetEmailCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<SendPasswordResetEmailCommandHandler> _logger;
    private readonly IMailSender _mailSender;
    private readonly ApplicationDomain _applicationDomain;

    public SendPasswordResetEmailCommandHandler(UserManager<ApplicationUser> userManager,
        IMailSender mailSender,
        ILogger<SendPasswordResetEmailCommandHandler> logger,
        IOptions<ApplicationDomain> applicationOptions)
    {
        _userManager = userManager;
        _mailSender = mailSender;
        _logger = logger;
        _applicationDomain = applicationOptions.Value;
    }

    public async Task<Result> Handle(SendPasswordResetEmailCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email)
                ;
            if (user == null)
                return Result.Failure(DomainErrors.User.NotFound(request.Email));

            if(!user.EmailConfirmed)
                return Result.Failure(DomainErrors.Auth.EmailNotConfirmed);

            _logger.LogInformation("Generating password reset token for user with email: {Email}", request.Email);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var template = await File.ReadAllTextAsync("EmailTemplates\\PasswordResetTemplate.html");
            template = template.Replace("{{ResetLink}}", $"{_applicationDomain}/api/auth/reset-password?token={token}&email={user.Email}");

            await _mailSender.SendAsync(
                to: request.Email,
                subject: "Password Reset Request",
                body: template
            );

            _logger.LogInformation("Password reset email sent to user with email: {Email}", request.Email);
            return Result.Success();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error occurred while sending password reset email to {Email}", request.Email);
            throw;
        }
    }
}
