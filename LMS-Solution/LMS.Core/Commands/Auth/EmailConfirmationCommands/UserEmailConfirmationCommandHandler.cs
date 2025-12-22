using LMS.Domin.Entities;
using LMS.Domin.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Commands.Auth.EmailConfirmationCommands;

public class UserEmailConfirmationCommandHandler : IRequestHandler<UserEmailConfirmationCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserEmailConfirmationCommandHandler> _logger;

    public UserEmailConfirmationCommandHandler(ILogger<UserEmailConfirmationCommandHandler> logger, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public async Task Handle(UserEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email)
            ?? throw new ResourceNotFoundException("User", request.Email);

        if(user.EmailConfirmed)
        {
            _logger.LogInformation("Email already confirmed for user with email {Email}", request.Email);
            return;
        }

        _logger.LogInformation("Confirming email for user with email {Email}", request.Email);

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Email confirmation failed for user with email {Email}. Errors: {Errors}", request.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
            throw new EmailConfirmationException("Email confirmation failed. Please ensure the token is valid and has not expired.");
        }

        return;
    }
}
