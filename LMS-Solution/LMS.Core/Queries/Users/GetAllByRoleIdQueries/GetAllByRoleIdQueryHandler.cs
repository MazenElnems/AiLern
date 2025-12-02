using AutoMapper;
using LMS.Domin.Contracts;
using LMS.Domin.DTOs.Users;
using LMS.Domin.Entities;
using LMS.Domin.Exceptions;
using MediatR;

namespace LMS.Core.Queries.Users.GetAllByRoleIdQueries;

public class GetAllByRoleIdQueryHandler : IRequestHandler<GetAllByRoleIdQuery, List<GetUsersByRoleDto>>
{
    private readonly IUsersRepository _userManagementRepository;
    private readonly IMapper _mapper;

    public GetAllByRoleIdQueryHandler(IUsersRepository userManagementRepository, IMapper mapper)
    {
        _userManagementRepository = userManagementRepository;
        _mapper = mapper;
    }

    public async Task<List<GetUsersByRoleDto>> Handle(GetAllByRoleIdQuery request, CancellationToken cancellationToken)
    {
        var users = await _userManagementRepository
            .GetUsersByRoleIdAsync(request.RoleId, request.SortBy, request.Order, request.PageNumber, request.PageSize)
            ?? throw new ResourceNotFoundException(nameof(ApplicationUser), request.RoleId.ToString());

        var dto = _mapper.Map<List<GetUsersByRoleDto>>(users);
        return dto;
    }
}