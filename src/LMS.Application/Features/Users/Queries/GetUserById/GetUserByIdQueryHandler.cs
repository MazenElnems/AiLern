using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.DTOs.Users;
using LMS.Domain.Common.Errors;
using LMS.Domain.Entities.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LMS.Application.Features.Users.Queries.GetUserById;


public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<GetUserByIdDto>>
{
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IMapper _mapper;


    public GetUserByIdQueryHandler(IUserStore<ApplicationUser> userStore, IMapper mapper)
    {
        _userStore = userStore;
        _mapper = mapper;
    }

    public async Task<Result<GetUserByIdDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userStore.FindByIdAsync(request.Id.ToString(), cancellationToken)
            ;
        if (user == null)
            return Result<GetUserByIdDto>.Failure(DomainErrors.User.NotFound(request.Id.ToString()));
        var dto = _mapper.Map<GetUserByIdDto>(user);
        return Result<GetUserByIdDto>.Success(dto);
        
    }
}
