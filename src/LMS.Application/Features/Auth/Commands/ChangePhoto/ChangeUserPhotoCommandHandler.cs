using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Users;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LMS.Application.Features.Auth.Commands.ChangePhoto;

public class ChangeUserPhotoCommandHandler : IRequestHandler<ChangeUserPhotoCommand, Result<string>>
{
    private readonly IWasabiService _wasabiService;
    private readonly IUserContext _user;
    private readonly UserManager<ApplicationUser> _userManager;



    public ChangeUserPhotoCommandHandler(IWasabiService wasabiService, IUserContext user, UserManager<ApplicationUser> userManager)
    {
        _wasabiService = wasabiService;
        _user = user;
        _userManager = userManager;
    }

    public async Task<Result<string>> Handle(ChangeUserPhotoCommand request, CancellationToken cancellationToken)
    {
        var userid = _user.GetCurrentUser().Id;
        var user = await _userManager.FindByIdAsync(userid.ToString());

        if (request.Image != null && !string.IsNullOrEmpty(user.ImageStoragePath))
        {
            var isExist = await _wasabiService.FileExists(user.ImageStoragePath, false);
            if (isExist)
            {
                await _wasabiService.DeleteFileAsync(user.ImageStoragePath, cancellationToken, false);
            }
        }
        string? key = null;
        string? url = null;
        if (request.Image != null && !request.Image.ContentType.StartsWith("image/"))
        {
            return DomainErrors.Common.BusinessRule("Invalid Image", "The uploaded file must be an image.");
        }
        if (request.Image != null)
        {
            key = $"users/{user.Id}/photo/{Guid.NewGuid()}.{request.Image.FileName.Split('.').Last()}";
            url = await _wasabiService.GeneratePresignedUploadUrlAsync(key, request.Image.ContentType, 15, secret: false);
        }
        user.ImageStoragePath = key;
        await _userManager.UpdateAsync(user);
        return url;
    }
}
