using LMS.Core.Commands.Users.DeleteUserRoleCommands;
using LMS.Core.Queries.Users.GetAllByRoleIdQueries;
using LMS.Shared.DTOs.Users;
using LMS.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("roles/{roleid}")]
        //[Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<IEnumerable<GetUsersByRoleDto>>> GetUsersByRoleId(int roleid,[FromQuery] GetAllByRoleIdQuery query)
        {
            query.RoleId = roleid;
            var dto = await _mediator.Send(query);
            return dto;
        }
        [HttpDelete("{id}/roles")]
        //[Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> DeleteUserRole(int id, DeleteUserRoleCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
