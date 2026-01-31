using LMS.Application.Common.Results;
using LMS.Domain.Common.Errors;
using LMS.Domain.Repositories;
using LMS.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Auth.Commands.ResendConfirmEmail;

public class ResendEmailConfirmationCommandHandler : IRequestHandler<ResendEmailConfirmationCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ResendEmailConfirmationCommandHandler> _logger;
    private readonly IMailSender _emailSender;

    public ResendEmailConfirmationCommandHandler(IMailSender emailSender, ILogger<ResendEmailConfirmationCommandHandler> logger, UserManager<ApplicationUser> userManager)
    {
        _emailSender = emailSender;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<Result> Handle(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email)
                ;
            if (user == null)
                return Result.Failure(DomainErrors.User.NotFound(request.Email));

            _logger.LogInformation("Resending email confirmation to user with email: {Email}", request.Email);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var template = await File.ReadAllTextAsync("EmailTemplates\\ConfirmationEmail.html");
            var html = template
                .Replace("{{ConfirmationLink}}", $"https://localhost:7080/api/auth/email-confirm?token={token}&userId={user.Id}");

            await _emailSender.SendAsync(request.Email, "Email Confirmation", html);
            return Result.Success();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while resending email confirmation to user with email: {Email}", request.Email);
            throw;
        }
    }
}