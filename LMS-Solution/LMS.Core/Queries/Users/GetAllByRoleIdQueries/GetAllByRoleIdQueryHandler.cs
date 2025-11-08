using AutoMapper;
using LMS.Core.Domain.Entities;
using LMS.Core.Domain.RepositoriesInterfaces;
using LMS.Core.Exceptions;
using LMS.Shared.Domain.Entities;
using LMS.Shared.DTOs.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Queries.Users.GetAllByRoleIdQueries
{
    internal class GetAllByRoleIdQueryHandler : IRequestHandler<GetAllByRoleIdQuery, List<GetUsersByRoleDto>>
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
                .GetUsersByRoleIdAsync(request.RoleId, request.SortBy, request.Order,request.PageNumber,request.PageSize)
                ?? throw new ResourceNotFoundException(nameof(ApplicationUser), request.RoleId.ToString());

            var dto =  _mapper.Map<List<GetUsersByRoleDto>>(users);
            return dto;
        }
    }
}
