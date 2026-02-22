using LMS.Application.Common.Results;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using LMS.Domain.Entities.Users;
using LMS.Domain.Errors;

namespace LMS.Application.Features.Auth.Commands.ResetPassword;

public class UserPasswordResetCommandHandler : IRequestHandler<UserPasswordResetCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserPasswordResetCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public UserPasswordResetCommandHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<UserPasswordResetCommandHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UserPasswordResetCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting password reset for user with email {Email}", request.Email);

            var user  = await _userManager.FindByEmailAsync(request.Email)
                ;
            if (user == null)
                return Result.Failure(DomainErrors.User.NotFound(request.Email));

            if (await _userManager.CheckPasswordAsync(user, request.NewPassword))
                return Result.Failure(DomainErrors.Auth.PasswordResetFailed);

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

            if(!result.Succeeded)
            {
                _logger.LogWarning("Password reset failed for user with email {Email}: {Errors}",
                    request.Email, string.Join(", ", result.Errors.Select(e => e.Description)));

                return Result.Failure(DomainErrors.Auth.PasswordResetFailed);
            }

            await _unitOfWork.Users.RevokeRefreshTokensByUserIdAsync(user.Id);
            return Result.Success();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while resetting password for user with email {Email}", request.Email);
            throw;
        }
    }
}
