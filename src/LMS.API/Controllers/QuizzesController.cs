using LMS.API.Controllers.Common;
using LMS.API.Models;
using LMS.Application.Features.Quizzes.Commands.DeleteQuiz;
using LMS.Application.Features.Quizzes.Commands.UpdateQuiz;
using LMS.Domain.Constants;
using LMS.Application.Features.Quizzes.Commands.CreateQuiz;
using LMS.Application.Features.Quizzes.Queries.GetAllQuizzes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuizzesController : ApiBaseController
{
    private readonly IMediator _mediator;

    public QuizzesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse>> Create(CreateQuizCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = UserRoles.Instructor)]
    [SwaggerOperation(Summary = "Update quiz", Description = "Updates an existing quiz.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Assignment updated successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Assignment not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> Update(Guid id, UpdateQuizCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = UserRoles.Instructor)]
    [SwaggerOperation(Summary = "Delete quiz", Description = "Deletes an quiz.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Quiz deleted successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Assignment not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteQuizCommand(id));
        return HandleResponse(this, result);
    }

    [HttpGet("courses/{courseId}/quizzes")]
    public async Task<ActionResult<ApiResponse>> GetAllQuizzesByCourseId(int courseId,[FromQuery] GetAllQuizzesByCourseIdQuery query)
    {
        query.CourseId = courseId;
        var result = await _mediator.Send(query);
        return HandleResponse(this, result);
    }
}
