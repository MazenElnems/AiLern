using LMS.Domain.DTOs.Users;
using MediatR;

namespace LMS.Application.Queries.Users.GetByIdQueries;

public class GetUserByIdQuery(int id) : IRequest<GetUserByIdDto>
{
    
    public int Id { get; } = id;
}
