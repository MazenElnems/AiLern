using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Users.DTO;
using MediatR;

namespace LMS.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQuery(int id) : IRequest<Result<GetUserByIdDto>>
{
    
    public int Id { get; } = id;
}
