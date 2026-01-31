using LMS.API.Common.Responses;
using LMS.API.Controllers.Common;
using LMS.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LMS.Application.Features.AssignmentSubmissions.Commands.Submit;
using LMS.Application.Features.AssignmentSubmissions.Commands.RequestPreSignedUrl;
using LMS.Application.Features.AssignmentSubmissions.Commands.ConfirmUpload;
using LMS.Application.Features.AssignmentSubmissions.Commands.DeleteSubmission;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentSubmissionsController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public AssignmentSubmissionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{id}/presigned-url")]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<ActionResult<ApiResponse>> RequestSubmissionUploade(int id, RequestSubmissionPresignedUrlCommand command)
        {
            command.SubmissionId = id;

            var result = await _mediator.Send(command);
            return HandleResponse(this, result);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<ActionResult<ApiResponse>> Create(SubmissionCreateCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResponse(this, result);
        }

        [HttpPost("{id}/confirm-upload")]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<ActionResult<ApiResponse>> ConfirmSubmissionUploadAsync(int id)
        {
            var result = await _mediator.Send(new ConfirmSubmissionUploadCommand { SubmissionId = id });
            return HandleResponse(this, result);

        }

        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<ActionResult<ApiResponse>> Delete(int id)
        {
            var result = await _mediator.Send(new SubmissionDeleteCommand(id));
            return HandleResponse(this, result);

        }


    }
}
