using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Users;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LMS.Application.Features.Auth.Commands.ChangePassword;

internal class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public ChangePasswordCommandHandler(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userEmail  = _userContext.GetCurrentUser().Email;
        var user = await _userManager.FindByEmailAsync(userEmail);

        if (user == null)
            return DomainErrors.User.NotFound(userEmail);

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
            return DomainErrors.Auth.ChangePasswordFailed(string.Join(", ", result.Errors.Select(err => err.Description)));

        await _unitOfWork.Users.RevokeRefreshTokensByUserIdAsync(user.Id);

        return Result.Success("changed password successfully");
    }
}
