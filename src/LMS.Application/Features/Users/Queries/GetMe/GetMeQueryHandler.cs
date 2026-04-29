using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Users.Shared.DTO;
using LMS.Application.Settings;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LMS.Application.Features.Users.Queries.GetMe;

public class GetMeQueryHandler : IRequestHandler<GetMeQuery, Result<GetMeDto>>
{
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly BunnyOptions _bunnyOptions;

    public GetMeQueryHandler(IUserContext userContext, IUnitOfWork unitOfWork, IOptions<BunnyOptions> options)
    {
        _userContext = userContext;
        _unitOfWork = unitOfWork;
        _bunnyOptions = options.Value;
    }

    public async Task<Result<GetMeDto>> Handle(GetMeQuery request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();

        var userData = await _unitOfWork.Users.Query
            .Where(u => u.Id == user.Id)
            .Select(u=> new GetMeDto
            {
                Id = u.Id,
                FullName = u.FullName,
                UserName = u.UserName!,
                Role = u.Role,
                ImageUrl = u.ImageStoragePath
            }).FirstOrDefaultAsync();

        if (userData is null)
            return DomainErrors.User.NotFound(user.Id.ToString());

        if (!string.IsNullOrEmpty(userData.ImageUrl))
        {
            userData.ImageUrl = $"{_bunnyOptions.PublicUrl}{userData.ImageUrl}";
        }

        return userData;
    }
}
