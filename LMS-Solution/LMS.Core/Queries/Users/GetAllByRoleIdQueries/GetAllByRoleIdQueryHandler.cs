using AutoMapper;
using LMS.Domin.Contracts;
using LMS.Domin.DTOs;
using LMS.Domin.DTOs.Users;
using LMS.Domin.Entities;
using LMS.Domin.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LMS.Core.Queries.Users.GetAllByRoleIdQueries;

public class GetAllByRoleIdQueryHandler : IRequestHandler<GetAllByRoleIdQuery, PaginationResult<GetUsersByRoleDto>>
{
    private readonly IUsersRepository _userRepository;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly IMapper _mapper;

    public GetAllByRoleIdQueryHandler(IUsersRepository userManagementRepository, IMapper mapper, RoleManager<IdentityRole<int>> roleManager)
    {
        _userRepository = userManagementRepository;
        _mapper = mapper;
        _roleManager = roleManager;
    }

    public async Task<PaginationResult<GetUsersByRoleDto>> Handle(GetAllByRoleIdQuery request, CancellationToken cancellationToken)
    {
        if(await _roleManager.FindByIdAsync(request.RoleId.ToString()) == null)
            throw new ResourceNotFoundException("Role", request.RoleId.ToString());

        var (users, totalResult) = await _userRepository
            .GetUsersByRoleIdAsync(request.RoleId, request.SortBy, request.Order, request.PageNumber, request.PageSize);

        var dto = _mapper.Map<List<GetUsersByRoleDto>>(users);

        return new PaginationResult<GetUsersByRoleDto>(request.PageNumber, request.PageSize, totalResult, dto);
    }
}