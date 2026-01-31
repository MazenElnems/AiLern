using LMS.API.Common.Responses;
using LMS.API.Controllers.Common;
using LMS.Application.Features.Assignments.Commands.ConfirmFileUpload;
using LMS.Application.Features.Assignments.Commands.CreateAssignment;
using LMS.Application.Features.Assignments.Commands.DaleteAssignmentFile;
using LMS.Application.Features.Assignments.Commands.DeleteAssignment;
using LMS.Application.Features.Assignments.Commands.PresignedUrlFileUpload;
using LMS.Application.Features.Assignments.Commands.PublishAssignment;
using LMS.Application.Features.Assignments.Commands.UpdateAssignment;
using LMS.Application.Features.Assignments.Queries.GetAssignment;
using LMS.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AssignmentsController : ApiBaseController
{
    private readonly IMediator _mediator;

    public AssignmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<ActionResult<ApiResponse>> Create(AssignmentCreateCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpPost("presigned-url")]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<ActionResult<ApiResponse>> RequestAssignmentUpload(RequestPreSignedUrlCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpPost("confirm-upload")]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<ActionResult<ApiResponse>> ConfirmAssignmentUpload(ConfirmAssignmentUploadCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpPost("{id}/publish")]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<ActionResult<ApiResponse>> Publish(int id)
    {
        var result = await _mediator.Send(new AssignmentPublishCommand(id));
        return HandleResponse(this, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<ActionResult<ApiResponse>> Update(int id, AssignmentUpdateCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<ActionResult<ApiResponse>> Delete(int id)
    {
        var result = await _mediator.Send(new AssignmentDeleteCommand(id));
        return HandleResponse(this, result);
    }

    [HttpDelete("{id}/files/{fileId}")]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<ActionResult<ApiResponse>> DeleteFile(int id, Guid fileId)
    {
        var result = await _mediator.Send(new AssignmentDeleteFileCommand(id, fileId));
        return HandleResponse(this, result);
    }

    [HttpGet("/courses/{courseId}/assignments/{id}")]
    [Authorize(Roles = UserRoles.Instructor + "," + UserRoles.Student)]
    public async Task<ActionResult<ApiResponse>> GetAssignment(int id, int courseId)
    {
        var result = await _mediator.Send(new GetAssignmentQuery(id, courseId));
        return HandleResponse(this, result);
    }
}