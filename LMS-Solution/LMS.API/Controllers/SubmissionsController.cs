using LMS.Core.Commands.Assignments.AssignmentCreateCommands;
using LMS.Core.Commands.Assignments.AssignmentDeleteCommands;
using LMS.Core.Commands.Submissions.SubmissionCreateCommands;
using LMS.Core.Commands.Submissions.SubmissionDeleteCommands;
using LMS.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubmissionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SubmissionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<IActionResult> Create(SubmissionCreateCommand command)
        {
            var entity = await _mediator.Send(command);
            return CreatedAtAction(nameof(Create), new { id = entity.Id }, entity);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new SubmissionDeleteCommand(id));
            return NoContent();
        }


    }
}
