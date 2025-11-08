using LMS.Core.DTOs.Users;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Queries.Users.GetByIdQueries
{
    public class GetUserByIdQuery(int id) : IRequest<GetUserByIdDto>
    {
        
        public int Id { get; } = id;
    }
}
