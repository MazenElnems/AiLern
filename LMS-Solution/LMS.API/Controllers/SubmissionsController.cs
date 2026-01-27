using LMS.Core.Commands.Assignments.ConfirmAssignmentUploadCommands;
using LMS.Core.Commands.Assignments.RequestPreSignedUrlCommands;
using LMS.Core.Commands.Submissions.ConfirmSubmissionUploadCommands;
using LMS.Core.Commands.Submissions.RequestSubmissionPresignedUrlCommands;
using LMS.Core.Commands.Assignments.AssignmentCreateCommands;
using LMS.Core.Commands.Assignments.AssignmentDeleteCommands;
using LMS.Core.Commands.Submissions.SubmissionCreateCommands;
using LMS.Core.Commands.Submissions.SubmissionDeleteCommands;
using LMS.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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

        [HttpPost("{id}/request-Upload")]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<IActionResult> RequestSubmissionUploade(int id, RequestSubmissionPresignedUrlCommand command)
        {
            command.SubmissionId = id;

            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<IActionResult> Create(SubmissionCreateCommand command)
        {
            var entity = await _mediator.Send(command);
            return CreatedAtAction(nameof(Create), new { id = entity.Id }, entity);
        }

        [HttpPost("{id}/confirm-upload")]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<IActionResult> ConfirmSubmissionUploadAsync(int id)
        {
            await _mediator.Send(new ConfirmSubmissionUploadCommand { SubmissionId = id });
            return NoContent();

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
