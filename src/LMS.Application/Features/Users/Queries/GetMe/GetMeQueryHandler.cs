using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Users.Shared.DTO;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Users.Queries.GetMe;

public class GetMeQueryHandler : IRequestHandler<GetMeQuery, Result<GetMeDto>>
{
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBunnyUrlSigner _bunny;
    public GetMeQueryHandler(IUserContext userContext, IUnitOfWork unitOfWork, IBunnyUrlSigner bunny)
    {
        _userContext = userContext;
        _unitOfWork = unitOfWork;
        _bunny = bunny;
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
            userData.ImageUrl = _bunny.GetUrl(userData.ImageUrl);
        }

        return userData;
    }
}
