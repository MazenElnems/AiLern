using LMS.Core.DTOs.Auth.Response;
using LMS.Domin.Entities;
using LMS.Domin.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Commands.Auth.EmailConfirmationCommands;

public class UserEmailConfirmationCommandHandler : IRequestHandler<UserEmailConfirmationCommand, EmailConfirmationResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserEmailConfirmationCommandHandler> _logger;

    public UserEmailConfirmationCommandHandler(ILogger<UserEmailConfirmationCommandHandler> logger, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<EmailConfirmationResponse> Handle(UserEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString())
            ?? throw new ResourceNotFoundException("User", request.UserId.ToString());

        _logger.LogInformation("Confirming email for user with ID {UserId}", request.UserId);

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Email confirmation failed for user with ID {UserId}. Errors: {Errors}", request.UserId, string.Join(", ", result.Errors.Select(e => e.Description)));
            return new EmailConfirmationResponse(false,string.Empty);
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        return new EmailConfirmationResponse(true, token, user.Id);
    }
}
