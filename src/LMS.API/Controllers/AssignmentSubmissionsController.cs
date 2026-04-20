using LMS.API.Controllers.Common;
using LMS.API.Models;
using LMS.Application.Features.AssignmentSubmissions.Commands.ConfirmUpload;
using LMS.Application.Features.AssignmentSubmissions.Commands.DeleteSubmission;
using LMS.Application.Features.AssignmentSubmissions.Commands.ReviewSubmission;
using LMS.Application.Features.AssignmentSubmissions.Commands.Submit;
using LMS.Application.Features.AssignmentSubmissions.Queries.GetStudentSubmissionsForAssignment;
using LMS.Application.Features.AssignmentSubmissions.Queries.GetSubmissionFiles;
using LMS.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.API.Controllers;

[Route("api/Assignments/Submissions")]
[ApiController]
[SwaggerTag("Assignment submission endpoints.")]
public class AssignmentSubmissionsController : ApiBaseController
{
    private readonly IMediator _mediator;

    public AssignmentSubmissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Student)]
    [SwaggerOperation(Summary = "Create submission", Description = "Creates a new assignment submission for the current student.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Submission created successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Assignment not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> Create(AssignmentSubmissionCreateCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = UserRoles.Student)]
    [SwaggerOperation(Summary = "Delete submission", Description = "Deletes a submission by ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Submission deleted successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Submission not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> Delete(int id)
    {
        var result = await _mediator.Send(new SubmissionDeleteCommand(id));
        return HandleResponse(this, result);
    }
    [HttpPut("{id}")]
    [Authorize(Roles = UserRoles.Instructor)]
    [SwaggerOperation(Summary = "Review submission", Description = "Review a submission by ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Submission reviewed successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Submission not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> Review(int id, SubmissionReviewCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpGet("/api/Assignments/{assignmentId}/Submissions")]
    [Authorize(Roles = UserRoles.Instructor)]
    [SwaggerOperation(Summary = "Get submissions for assignment", Description = "Lists student submissions for a specific assignment.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Submissions retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid query parameters.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Assignment not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetAllSubmissionsForAssignment(int assignmentId, string status, int pageNo = 1, int pageSize = 10)
    {
        var result = await _mediator.Send(new GetStudentSubmissionsForAssignmentQuery(assignmentId, status, pageNo, pageSize));
        return HandleResponse(this, result);
    }

    [HttpGet("/api/Assignments/{assignmentId}/Submissions/{submissionId}/files")]
    [Authorize(Roles = $"{UserRoles.Student},{UserRoles.Instructor}")]
    [SwaggerOperation(Summary = "Get submission files", Description = "Retrieves files for a specific submission.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Submission files retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Submission not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetSubmissionFiles(int assignmentId,int submissionId)
    {
        var result = await _mediator.Send(new GetAssignmentSubmissionFilesQuery(assignmentId,submissionId));
        return HandleResponse(this, result);
    }
}
