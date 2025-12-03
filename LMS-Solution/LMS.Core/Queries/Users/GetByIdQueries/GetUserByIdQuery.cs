using LMS.Domin.DTOs.Users;
using MediatR;

namespace LMS.Core.Queries.Users.GetByIdQueries;

public class GetUserByIdQuery(int id) : IRequest<GetUserByIdDto>
{
    
    public int Id { get; } = id;
}
