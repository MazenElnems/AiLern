using LMS.Application.Features.Courses.Commands.ProcessAIUploadHook;
using LMS.Application.Features.Courses.Shared.DTO;
using LMS.Application.Features.Quizzes.Commands.AddBatchQuestions;
using LMS.Application.Features.Quizzes.Shared.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/webhooks/ai")]
public class AIWebhookController : ControllerBase
{
    private readonly IMediator _mediator;

    public AIWebhookController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("upload-result")]
    [AllowAnonymous]
    public async Task<IActionResult> UploadResult([FromBody] AIWebhookDto dto)
    {
        await _mediator.Send(new ProcessAIUploadWebhookCommand(dto));
        return Ok();
    }

    [HttpPost("question")]
    [AllowAnonymous]
    public async Task<IActionResult> AddQuestionsPatch([FromBody] AIQuestionsGeneratedWebhookDto dto)
    {
        var command = new AddBatchQuestionsByTypeCommand(
            dto.QuestionType,
            dto.GeneratedQuestions,
            dto.Questions,
            dto.Completed,
            dto.QuizId
        );

        var result = await _mediator.Send(command);

        if(!result.IsSuccess)
            return BadRequest("Failed to add questions.");

        return Ok();
    }
}
