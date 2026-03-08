using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Domain.Constants;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
using LMS.Application.Common.Models.Responses;
using LMS.Domain.Entities.Users;
using LMS.Domain.Errors;
using LMS.Application.Features.Users.Shared.DTO;

namespace LMS.Application.Features.Users.Queries.GetAllByRoleId;

public class GetAllByRoleIdQueryHandler : IRequestHandler<GetAllByRoleIdQuery, Result<PaginationResult<GetUsersByRoleDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly IMapper _mapper;

    public GetAllByRoleIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, RoleManager<IdentityRole<int>> roleManager)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _roleManager = roleManager;
    }

    public async Task<Result<PaginationResult<GetUsersByRoleDto>>> Handle(GetAllByRoleIdQuery request, CancellationToken cancellationToken)
    {
        if (request.PageNumber < 1 || request.PageSize < 1)
        {
            return Result<PaginationResult<GetUsersByRoleDto>>.Failure(DomainErrors.Pagination.InvalidParameters);
        }

        var role = await _roleManager.FindByIdAsync(request.RoleId.ToString())
            ;
        if (role == null)
            return Result<PaginationResult<GetUsersByRoleDto>>.Failure(DomainErrors.Role.NotFound(request.RoleId.ToString()));

        Expression<Func<ApplicationUser, bool>> predicate = u => u.Role == role.Name;

        var sortBy = request.SortBy;
        var order = request.Order?.ToLower();
        var isDescending = order != SortOrderOptions.ASC;

        Expression<Func<ApplicationUser, object>> orderBy = sortBy?.ToLower() switch
        {
            var s when s == UserManagementSortByOptions.FullName.ToLower() => u => u.FullName,
            var s when s == UserManagementSortByOptions.UserName.ToLower() => u => u.UserName!,
            _ => u => u.UserName!
        };

        var totalResult = await _unitOfWork.Users.CountAsync(predicate);
        var users = await _unitOfWork.Users.FilterAsync(
            predicate,
            orderBy,
            isDescending,
            (request.PageNumber - 1) * request.PageSize,
            request.PageSize);

        var dto = _mapper.Map<List<GetUsersByRoleDto>>(users);
        return Result<PaginationResult<GetUsersByRoleDto>>.Success(
            new PaginationResult<GetUsersByRoleDto>(request.PageNumber, request.PageSize, totalResult, dto));
    }
}