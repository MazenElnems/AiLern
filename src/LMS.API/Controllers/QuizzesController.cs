using AutoMapper;
using LMS.API.Controllers.Common;
using LMS.API.Models;
using LMS.API.Models.Requests;
using LMS.Application.Features.Attempts.Commands.GradeAttemptByAI;
using LMS.Application.Features.Attempts.Queries.GetAttemptsByQuizId;
using LMS.Application.Features.Quizzes.Commands.AcceptAiGeneratedQuestion;
using LMS.Application.Features.Quizzes.Commands.AcceptAllAiGeneratedQuestions;
using LMS.Application.Features.Quizzes.Commands.CreateQuiz;
using LMS.Application.Features.Quizzes.Commands.DeleteQuiz;
using LMS.Application.Features.Quizzes.Commands.QenerateQuestionsUsingAI;
using LMS.Application.Features.Quizzes.Commands.RejectAiGeneratedQuestion;
using LMS.Application.Features.Quizzes.Commands.UpdateQuiz;
using LMS.Application.Features.Quizzes.Commands.UpdateQuizStatus;
using LMS.Application.Features.Quizzes.Commands.UpsertQuestions;
using LMS.Application.Features.Quizzes.Queries.GetAllQuizzes;
using LMS.Application.Features.Quizzes.Queries.GetPendingAiGeneratedQuestions;
using LMS.Application.Features.Quizzes.Queries.GetQuiz;
using LMS.Application.Features.Quizzes.Queries.GetSubmissionsByQuizId;
using LMS.Application.Features.Quizzes.Shared.Requests;
using LMS.Domain.Constants;
using LMS.Domain.Enums;
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
    private readonly IMapper _mapper;

    public QuizzesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost("/api/courses/{courseId}/quizzes")]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<ActionResult<ApiResponse>> Create(int courseId, CreateQuizCommand command)
    {
        command.CourseId = courseId;
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
        command.QuizId = id;
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpPut("{id}/update-status")]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<ActionResult<ApiResponse>> UpdateStatus(Guid id, QuizStatus status)
    {
        var result = await _mediator.Send(new UpdateQuizStatusCommand(id, status));
        return HandleResponse(this, result);
    }

    [HttpPut("{id}/questions")]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<ActionResult<ApiResponse>> UpsertQuestions(Guid id, [FromBody] List<QuestionUpsertRequest> questions)
    {
        var result = await _mediator.Send(new UpsertQuestionsCommand(id, questions));
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

    [HttpGet("/api/courses/{courseId}/quizzes")]
    [Authorize(Roles = $"{UserRoles.Instructor},{UserRoles.Student}")]
    public async Task<ActionResult<ApiResponse>> GetAllQuizzesByCourseId(int courseId, int pageNo = 1, int pageSize = 10)
    {
        var result = await _mediator.Send(new GetAllQuizzesByCourseIdQuery(courseId, pageNo, pageSize));
        return HandleResponse(this, result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = $"{UserRoles.Instructor},{UserRoles.Student}")]
    [SwaggerOperation(Summary = "Get quiz", Description = "Get an quiz by id.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Quiz retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "quiz not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetQuizById(Guid id)
    {
        var result = await _mediator.Send(new GetQuizByIdQuery(id));
        return HandleResponse(this, result);
    }

    [HttpGet("{id}/ai-generated-questions")]
    [Authorize(Roles = UserRoles.Instructor)]
    [SwaggerOperation(Summary = "List pending AI-generated questions", Description = "Returns AI-generated questions that are not yet accepted.")]
    public async Task<ActionResult<ApiResponse>> GetPendingAiGeneratedQuestions(Guid id)
    {
        var result = await _mediator.Send(new GetPendingAiGeneratedQuestionsQuery(id));
        return HandleResponse(this, result);
    }

    [HttpPut("{id}/ai-generated-questions/accept-all")]
    [Authorize(Roles = UserRoles.Instructor)]
    [SwaggerOperation(Summary = "Accept all pending AI questions")]
    public async Task<ActionResult<ApiResponse>> AcceptAllAiGeneratedQuestions(Guid id)
    {
        var result = await _mediator.Send(new AcceptAllAiGeneratedQuestionsCommand(id));
        return HandleResponse(this, result);
    }

    [HttpPut("{id}/ai-generated-questions/{questionId}/accept")]
    [Authorize(Roles = UserRoles.Instructor)]
    [SwaggerOperation(Summary = "Accept one AI-generated question")]
    public async Task<ActionResult<ApiResponse>> AcceptAiGeneratedQuestion(Guid id, Guid questionId)
    {
        var result = await _mediator.Send(new AcceptAiGeneratedQuestionCommand(id, questionId));
        return HandleResponse(this, result);
    }

    [HttpDelete("{id}/ai-generated-questions/{questionId}/reject")]
    [Authorize(Roles = UserRoles.Instructor)]
    [SwaggerOperation(Summary = "Reject and delete a pending AI-generated question")]
    public async Task<ActionResult<ApiResponse>> RejectAiGeneratedQuestion(Guid id, Guid questionId)
    {
        var result = await _mediator.Send(new RejectAiGeneratedQuestionCommand(id, questionId));
        return HandleResponse(this, result);
    }

    [HttpPost("{quizId}/generate-by-ai")]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<ActionResult<ApiResponse>> GenerateByAi(Guid quizId, GenerateQuestionByAIRequest request)
    {
        var command = new GenerateQuestionsCommand(
            quizId,
            request.FileIds,
            request.Topics,
            request.QuestionsCount,
            request.QuestionTypeCounts,
            request.QuestionDifficultyPercents,
            request.Query
        );

        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpGet("{id}/submissions")]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<ActionResult<ApiResponse>> GetSubmissionsByQuizId(Guid id, AttemptStatus? status, int pageNo = 1, int pageSize = 10)
    {
        var result = await _mediator.Send(new GetSubmissionsByQuizIdQuery(id, pageNo, pageSize, status));
        return HandleResponse(this, result);
    }

    [HttpGet("{id}/my-attempts")]
    [Authorize(Roles = UserRoles.Student)]
    public async Task<ActionResult<ApiResponse>> GetAttemptsByQuizId(Guid id)
    {
        var result = await _mediator.Send(new GetAttemptsByQuizIdQuery(id));
        return HandleResponse(this, result);
    }

    [SwaggerOperation(Summary = "Put attempt", Description = "Put an attempt.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Attempts will be graded successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "quiz not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    [HttpPost("{id}/ai-grade")]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<ActionResult<ApiResponse>> GradeSubmissionsByAI(Guid id, [FromBody] AIGradingRequest request)
    {
        var result = await _mediator.Send(new GradeByAICommand(id, request.AttemptIds));
        return HandleResponse(this, result);
    }
}
