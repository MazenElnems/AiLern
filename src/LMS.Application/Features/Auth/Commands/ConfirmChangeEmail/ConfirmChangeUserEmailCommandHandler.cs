using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Users;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace LMS.Application.Features.Auth.Commands.ConfirmChangeEmail;

public class ConfirmChangeUserEmailCommandHandler : IRequestHandler<ConfirmChangeUserEmailCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserContext _user;
    private readonly IUnitOfWork _unitOfWork;


    public ConfirmChangeUserEmailCommandHandler(UserManager<ApplicationUser> userManager, IUserContext user, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _user = user;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ConfirmChangeUserEmailCommand request, CancellationToken cancellationToken)
    {

        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            return DomainErrors.User.NotFound(request.UserId.ToString());

        var decodedToken = Encoding.UTF8.GetString(
            WebEncoders.Base64UrlDecode(request.Token)
        );

        var result = await _userManager.ChangeEmailAsync(user, request.NewEmail, decodedToken);

        if (!result.Succeeded)
            return DomainErrors.User.CreationFailed(string.Join(", ", result.Errors.Select(e => e.Description)));
        await _unitOfWork.Users.RevokeRefreshTokensByUserIdAsync(user.Id);


        return Result.Success("Email changed successfully");
    }
}
