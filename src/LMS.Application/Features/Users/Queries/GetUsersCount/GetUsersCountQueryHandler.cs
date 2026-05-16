using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.Features.Users.Shared.DTO;
using LMS.Domain.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Users.Queries.GetUsersCount;

public class GetUsersCountQueryHandler : IRequestHandler<GetUsersCountQuery, Result<GetUsersCountDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUsersCountQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GetUsersCountDto>> Handle(GetUsersCountQuery request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.Query.Where(u=>true).ToListAsync();

        var dto = new GetUsersCountDto
        {
            TotalUsers = users.Count,
            TotalStudent = users.Where(u => u.Role == UserRoles.Student).Count(),
            TotalInstructors = users.Where(u => u.Role == UserRoles.Instructor).Count(),
            TotalAdmins = users.Where(u => u.Role == UserRoles.Admin).Count()
        };

        return dto;
    }
}
