using LMS.Core.Commands.Users.UpdateCommands;
using LMS.Core.Queries.Users.GetByIdQueries;
using LMS.Shared.DTOs.Users;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserManagementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<GetUserByIdDto>> GetById(int id)
        {
            var dto = await _mediator.Send(new GetUserByIdQuery(id));
            return dto;
        }
        [HttpPut]
        [Route("{id}/roles")]
        public async Task<IActionResult> AddRole(int id, AddUserToRoleCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();

        }
    }
}
