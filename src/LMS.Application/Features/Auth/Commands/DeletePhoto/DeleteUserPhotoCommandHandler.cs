using LMS.Application.Common.Results;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.Services;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LMS.Application.Features.Auth.Commands.DeletePhoto;

public class DeleteUserPhotoCommandHandler : IRequestHandler<DeleteUserPhotoCommand, Result>
{
    private readonly IWasabiService _wasabiService;
    private readonly IUserContext _user;
    private readonly UserManager<ApplicationUser> _userManager;

    public DeleteUserPhotoCommandHandler(IWasabiService wasabiService, IUserContext user, UserManager<ApplicationUser> userManager)
    {
        _wasabiService = wasabiService;
        _user = user;
        _userManager = userManager;
    }

    public async Task<Result> Handle(DeleteUserPhotoCommand request, CancellationToken cancellationToken)
    {
        var userid = _user.GetCurrentUser().Id;
        var user = await _userManager.FindByIdAsync(userid.ToString());
        if (!string.IsNullOrEmpty(user.ImageStoragePath))
        {
            var isExist = await _wasabiService.FileExists(user.ImageStoragePath, false);
            if (isExist)
            {
                await _wasabiService.DeleteFileAsync(user.ImageStoragePath, cancellationToken, false);
            }
        }
        user.ImageStoragePath = null;
        await _userManager.UpdateAsync(user);
        return Result.Success();
    }
}
