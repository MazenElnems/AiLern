using LMS.API.Controllers.Common;
using LMS.API.Models;
using LMS.Application.Features.Attempts.Commands.CreateAttempt;
using LMS.Application.Features.Attempts.Commands.SubmitAttempt;
using LMS.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttemptsController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public AttemptsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("/api/quizzes/{quizId}/[controller]")]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<ActionResult<ApiResponse>> Create([FromRoute] Guid quizId)
        {
            var command = new CreateAttemptCommand(quizId);
            var result = await _mediator.Send(command);
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
    }
}
