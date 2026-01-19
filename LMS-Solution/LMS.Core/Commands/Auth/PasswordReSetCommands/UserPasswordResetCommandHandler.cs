using LMS.Domin.Repositories;
using LMS.Domin.Entities;
using LMS.Domin.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Commands.Auth.PasswordReSetCommands;

public class UserPasswordResetCommandHandler : IRequestHandler<UserPasswordResetCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserPasswordResetCommandHandler> _logger;
    private readonly IUsersRepository _usersRepository;

    public UserPasswordResetCommandHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<UserPasswordResetCommandHandler> logger,
        IUsersRepository usersRepository)
    {
        _userManager = userManager;
        _logger = logger;
        _usersRepository = usersRepository;
    }

    public async Task Handle(UserPasswordResetCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting password reset for user with email {Email}", request.Email);

            var user  = await _userManager.FindByEmailAsync(request.Email)
                ?? throw new ResourceNotFoundException(nameof(ApplicationUser), request.Email);

            if (await _userManager.CheckPasswordAsync(user, request.NewPassword))
                throw new PasswordResetException();

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

            if(!result.Succeeded)
            {
                _logger.LogWarning("Password reset failed for user with email {Email}: {Errors}",
                    request.Email, string.Join(", ", result.Errors.Select(e => e.Description)));

                throw new PasswordResetException();
            }

            await _usersRepository.RevokeRefreshTokensByUserIdAsync(user.Id);
        }
        catch (PasswordResetException ex)
        {
            throw;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while resetting password for user with email {Email}", request.Email);
            throw;
        }
    }
}
