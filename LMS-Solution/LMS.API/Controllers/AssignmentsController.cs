using LMS.Core.Commands.Assignments.AssignmentCreateCommands;
using LMS.Core.Commands.Assignments.AssignmentDeleteCommands;
using LMS.Core.Commands.Assignments.AssignmentDeleteFileCommands;
using LMS.Core.Commands.Assignments.AssignmentPublishCommands;
using LMS.Core.Commands.Assignments.ConfirmAssignmentUploadCommands;
using LMS.Core.Commands.Assignments.RequestPreSignedUrlCommands;
using LMS.Core.Commands.Assignments.AssignmentUpdateCommands;
using LMS.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LMS.Core.Commands.Assignments.GetAssignmentCommands;

namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AssignmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AssignmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<IActionResult> Create(AssignmentCreateCommand command)
    {
        var entity = await _mediator.Send(command);
        return CreatedAtAction(nameof(Create), new { id = entity.Id }, entity);
    }

    [HttpPost("presigned-url")]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<IActionResult> RequestAssignmentUpload(RequestPreSignedUrlCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpPost("confirm-upload")]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<IActionResult> ConfirmAssignmentUpload(ConfirmAssignmentUploadCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("{id}/publish")]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<IActionResult> Publish(int id)
    {
        await _mediator.Send(new AssignmentPublishCommand(id));
        return NoContent();
    }

    [HttpPut("{id}")]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<IActionResult> Update(int id, AssignmentUpdateCommand command)
    {
        command.Id = id;
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new AssignmentDeleteCommand(id));
        return NoContent();
    }

    [HttpDelete("{id}/files/{fileId}")]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<IActionResult> DeleteFile(int id, Guid fileId)
    {
        await _mediator.Send(new AssignmentDeleteFileCommand(id, fileId));
        return NoContent();
    }

    [HttpGet("/courses/{courseId}/assignments/{id}")]
    [Authorize(Roles = UserRoles.Instructor + "," + UserRoles.Student)]
    public async Task<IActionResult> GetAssignment(int id, int courseId)
    {
        var dto = await _mediator.Send(new GetAssignmentCommand(id, courseId));
        return Ok(dto);
    }
}