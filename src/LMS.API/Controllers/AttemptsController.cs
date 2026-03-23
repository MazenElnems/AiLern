using LMS.API.Controllers.Common;
using LMS.API.Models;
using LMS.Application.Features.Attempts.Commands.GradeSubmission;
using LMS.Application.Features.Attempts.Queries.GetAttempt;
using LMS.Application.Features.Attempts.Queries.GetAttemptInstructor;
using LMS.Application.Features.Attempts.Commands.CreateAttempt;
using LMS.Application.Features.Attempts.Commands.SaveAttempt;
using LMS.Application.Features.Attempts.Commands.SubmitAttempt;
using LMS.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AttemptsController : ApiBaseController
{
    private readonly IMediator _mediator;

    public AttemptsController(IMediator mediator)
    {
        this._mediator = mediator;
            _mediator = mediator;
    }

    [SwaggerOperation(Summary = "Get attempt", Description = "Get an attempt by id.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Attempt retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "quiz not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    [HttpGet("{id}/student")]
    [Authorize(Roles = UserRoles.Student)]
    public async Task<ActionResult<ApiResponse>> GetAttemptByIdForStudent(Guid id, [FromQuery] GetAttemptByIdForStudentQuery query)
    {
        query.Id = id;
        var result = await _mediator.Send(query);
        return HandleResponse(this, result);
    }

    [Authorize(Roles = UserRoles.Student)]
    [HttpPost("/api/quizzes/{quizId}/[controller]")]
    public async Task<ActionResult<ApiResponse>> Create([FromRoute] Guid quizId)
    {
        var command = new CreateAttemptCommand(quizId);
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }
    [SwaggerOperation(Summary = "Get attempt", Description = "Get an attempt by id.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Attempt retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "quiz not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    [HttpGet("{id}/instrudcor")]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<ActionResult<ApiResponse>> GetAttemptByIdForInstructor(Guid id, [FromQuery] GetAttemptByIdForInstructorQuery query)
    {
        query.Id = id;
        var result = await _mediator.Send(query);
        return HandleResponse(this, result);
    }

    [HttpPut("{attemptId}")]
    [Authorize(UserRoles.Student)]
    public async Task<ActionResult<ApiResponse>> Submit([FromRoute] Guid attemptId)
    {
        var command = new SubmitAttemptCommand(attemptId);
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [SwaggerOperation(Summary = "Put attempt", Description = "Put an attempt.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Attempt retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "quiz not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    [HttpPut("{id}/grade")]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<ActionResult<ApiResponse>> GradeSubmission(Guid id, [FromBody] GradeSubmissionCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpPost("{attemptId}/save")]
    [Authorize(Roles = UserRoles.Student)]
    public async Task<ActionResult<ApiResponse>> Save([FromRoute] Guid attemptId, [FromBody] SaveAttemptCommand command)
    {
        command.AttemptId = attemptId;
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }
}

