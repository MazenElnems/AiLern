using AutoMapper;
using LMS.Domin.Constants;
using LMS.Domin.Repositories;
using LMS.Domin.Entities;
using LMS.Domin.DTOs;
using LMS.Domin.DTOs.Users;
using LMS.Domin.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

namespace LMS.Core.Queries.Users.GetAllByRoleIdQueries;

public class GetAllByRoleIdQueryHandler : IRequestHandler<GetAllByRoleIdQuery, PaginationResult<GetUsersByRoleDto>>
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

    public async Task<PaginationResult<GetUsersByRoleDto>> Handle(GetAllByRoleIdQuery request, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(request.RoleId.ToString())
            ?? throw new ResourceNotFoundException("Role", request.RoleId.ToString());

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

        return new PaginationResult<GetUsersByRoleDto>(request.PageNumber, request.PageSize, totalResult, dto);
    }
}