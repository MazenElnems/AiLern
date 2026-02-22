using LMS.API.Controllers.Common;
using LMS.API.Models;
using LMS.Application.Features.Assignments.Commands.ConfirmFileUpload;
using LMS.Application.Features.Assignments.Commands.CreateAssignment;
using LMS.Application.Features.Assignments.Commands.DaleteAssignmentFile;
using LMS.Application.Features.Assignments.Commands.DeleteAssignment;
using LMS.Application.Features.Assignments.Commands.UpdateAssignment;
using LMS.Application.Features.Assignments.Queries.GetAssignment;
using LMS.Application.Features.Assignments.Queries.GetCourseAssignmentsForInstructors;
using LMS.Application.Features.Assignments.Queries.GetCourseAssignmentsForStudent;
using LMS.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[SwaggerTag("Assignment management endpoints.")]
public class AssignmentsController : ApiBaseController
{
    private readonly IMediator _mediator;

    public AssignmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Instructor)]
    [SwaggerOperation(Summary = "Create assignment", Description = "Creates a new assignment for a course.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Assignment created successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> Create(AssignmentCreateCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpPost("confirm-upload")]
    [Authorize(Roles = UserRoles.Instructor)]
    [SwaggerOperation(Summary = "Confirm assignment upload", Description = "Confirms uploaded assignment files and finalizes the assignment.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Assignment upload confirmed.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Assignment not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> ConfirmAssignmentUpload(ConfirmAssignmentUploadCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = UserRoles.Instructor)]
    [SwaggerOperation(Summary = "Update assignment", Description = "Updates an existing assignment.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Assignment updated successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Assignment not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> Update(int id, AssignmentUpdateCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = UserRoles.Instructor)]
    [SwaggerOperation(Summary = "Delete assignment", Description = "Deletes an assignment.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Assignment deleted successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Assignment not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> Delete(int id)
    {
        var result = await _mediator.Send(new AssignmentDeleteCommand(id));
        return HandleResponse(this, result);
    }

    [HttpDelete("{id}/files/{fileId}")]
    [Authorize(Roles = UserRoles.Instructor)]
    [SwaggerOperation(Summary = "Delete assignment file", Description = "Deletes a file associated with an assignment.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Assignment file deleted successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Assignment or file not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> DeleteFile(int id, Guid fileId)
    {
        var result = await _mediator.Send(new AssignmentDeleteFileCommand(id, fileId));
        return HandleResponse(this, result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = UserRoles.Instructor + "," + UserRoles.Student)]
    [SwaggerOperation(Summary = "Get assignment", Description = "Gets assignment details by ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Assignment retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Assignment not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetAssignment(int id)
    {
        var result = await _mediator.Send(new GetAssignmentQuery(id));
        return HandleResponse(this, result);
    }

    [HttpGet("/api/Courses/{courseId}/instructors/[controller]")]
    [Authorize(Roles = UserRoles.Instructor)]
    [SwaggerOperation(Summary = "Get course assignments for instructor", Description = "Lists assignments for a course (instructor view).")]
    [SwaggerResponse(StatusCodes.Status200OK, "Assignments retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetCourseAssignmentsForInstructor(int courseId)
    {
        var result = await _mediator.Send(new GetCourseAssignmentsForInstructorsQuery(courseId));
        return HandleResponse(this, result);
    }

    [HttpGet("/api/Courses/{courseId}/students/[controller]")]
    [Authorize(Roles = UserRoles.Student)]
    [SwaggerOperation(Summary = "Get course assignments for student", Description = "Lists assignments for a course (student view).")]
    [SwaggerResponse(StatusCodes.Status200OK, "Assignments retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetCourseAssignmentsForStudent(int courseId)
    {
        var result = await _mediator.Send(new GetCourseAssignmentsForStudentQuery(courseId));
        return HandleResponse(this, result);
    }
}