using AutoMapper;
using LMS.Core.Domain.Entities;
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

namespace LMS.Core.Queries.Users.GetByIdQueries
{

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, GetUserByIdDto>
    {
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IMapper _mapper;


        public GetUserByIdQueryHandler(IUserStore<ApplicationUser> userStore, IMapper mapper)
        {
            _userStore = userStore;
            _mapper = mapper;
        }




        public async Task<GetUserByIdDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userStore.FindByIdAsync(request.Id.ToString(), cancellationToken)
                ?? throw new ResourceNotFoundException(nameof(ApplicationUser), request.Id.ToString());
            var dto = _mapper.Map<GetUserByIdDto>(user);
            return dto;
            
        }
    }
}
