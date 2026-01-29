using LMS.Domain.Repositories;
using LMS.Domain.Entities;
using LMS.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Commands.Auth.ResendEmailConfirmationCommands;

public class ResendEmailConfirmationCommandHandler : IRequestHandler<ResendEmailConfirmationCommand>
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

    public async Task Handle(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email)
                ?? throw new ResourceNotFoundException("User", request.Email);

            _logger.LogInformation("Resending email confirmation to user with email: {Email}", request.Email);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var template = await File.ReadAllTextAsync("EmailTemplates\\ConfirmationEmail.html");
            var html = template
                .Replace("{{ConfirmationLink}}", $"https://localhost:7080/api/auth/email-confirm?token={token}&userId={user.Id}");

            await _emailSender.SendAsync(request.Email, "Email Confirmation", html);
        }
        catch(ResourceNotFoundException ex)
        {
            throw;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while resending email confirmation to user with email: {Email}", request.Email);
            throw;
        }
    }
}