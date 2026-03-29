using LMS.Application.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LMS.Domain.Entities.Users;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace LMS.Application.Features.Auth.Commands.ResetPassword;

public class UserPasswordResetCommandHandler : IRequestHandler<UserPasswordResetCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public UserPasswordResetCommandHandler(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UserPasswordResetCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Result.Failure(DomainErrors.User.NotFound(request.Email));

        // can't reset the same password
        if (await _userManager.CheckPasswordAsync(user, request.NewPassword))
            return DomainErrors.Auth.PasswordResetFailed;


        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));

        var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);

        if (!result.Succeeded)
            return DomainErrors.Auth.PasswordResetFailed;

        // enforce user to login after reset password
        await _unitOfWork.Users.RevokeRefreshTokensByUserIdAsync(user.Id);

        return Result.Success("reset password successfully");
    }
}
