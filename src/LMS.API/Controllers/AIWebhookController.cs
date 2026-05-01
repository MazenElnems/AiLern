using LMS.Application.Features.Courses.Commands.ProcessAIUpload;
using LMS.Application.Features.Courses.Shared.DTO;
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
}