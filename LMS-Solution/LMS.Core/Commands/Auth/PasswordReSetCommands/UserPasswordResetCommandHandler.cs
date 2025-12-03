using LMS.Domin.Entities;
using LMS.Domin.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace LMS.Core.Commands.Auth.PasswordReSetCommands;

public class UserPasswordResetCommandHandler : IRequestHandler<UserPasswordResetCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserPasswordResetCommandHandler> _logger;
    public UserPasswordResetCommandHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<UserPasswordResetCommandHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task Handle(UserPasswordResetCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting password reset for user with ID {UserId}", request.UserId);

            var user  = await _userManager.FindByIdAsync(request.UserId.ToString())
                ?? throw new ResourceNotFoundException(nameof(ApplicationUser), request.UserId.ToString());

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Password reset failed for user with ID {UserId}: {Errors}", 
                    request.UserId, string.Join(", ", result.Errors.Select(e => e.Description)));

                throw new WeakPasswordException(string.Join(',', result.Errors.Select(e => e.Description)));
            }
        }
        catch (WeakPasswordException ex)
        {
            throw;
        }
    }
}
