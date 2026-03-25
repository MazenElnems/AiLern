using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Users.Shared.DTO;
using LMS.Domain.Enums;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Users.Queries.GetAllByRoleId;

public class GetAllByRoleQueryHandler : IRequestHandler<GetAllByRoleQuery, Result<PaginationResult<GetUsersByRoleDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllByRoleQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginationResult<GetUsersByRoleDto>>> Handle(GetAllByRoleQuery request, CancellationToken cancellationToken)
    {
        bool invalidRole = 
            request.Role != Roles.Admin &&
            request.Role != Roles.Student &&
            request.Role != Roles.Instructor;

        if (invalidRole)
            return new PaginationResult<GetUsersByRoleDto>(
                request.PageNo,
                request.PageSize,
                0,
                []
            );

        var query = _unitOfWork.Users.Query
            .AsNoTracking();
            query = query.Where(u => u.Role ==  request.Role.ToString());

        var totalResult = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(request.PageSize * (request.PageNo - 1))
            .Take(request.PageSize)
            .Select(u => new GetUsersByRoleDto
            {
                Id = u.Id,
                Email = u.Email!,
                FullName = u.FullName,
                UserName = u.UserName!,
                PhoneNumber = u.PhoneNumber!,
                Role = u.Role
            })
            .ToListAsync();

        return new PaginationResult<GetUsersByRoleDto>(
            request.PageNo,
            request.PageSize,
            totalResult,
            items
        );
    }
}
