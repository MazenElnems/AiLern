using LMS.API.Controllers.Common;
using LMS.API.Models.Requests;
using LMS.API.Models.Responses;
using LMS.Application.Features.Courses.Commands.ConfirmAIResources;
using LMS.Application.Features.Courses.Commands.DeleteAIResources;
using LMS.Application.Features.Courses.Commands.ProcessAIDocs;
using LMS.Application.Features.Courses.Commands.UploadAIResources;
using LMS.Application.Features.Courses.Queries.GetAIResources;
using LMS.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.API.Controllers
{
    [Route("api/Courses/{id}/ai-resources")]
    [ApiController]
    public class AIResourcesController(IMediator mediator) : ApiBaseController
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        [Authorize(Roles = UserRoles.Instructor)]
        [SwaggerOperation(Summary = "Upload AI resources", Description = "Uploads AI resources for a course.")]
        [SwaggerResponse(StatusCodes.Status200OK, "AI resources uploaded successfully   .", typeof(ApiResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.", typeof(ApiResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
        public async Task<ActionResult<ApiResponse>> UploadAIResources(int id, UploadAIResourcesCommand command)
        {
            command.CourseId = id;
            var result = await _mediator.Send(command);
            return HandleResponse(this, result);
        }

        [HttpPut("confirm")]
        [Authorize(Roles = UserRoles.Instructor)]
        [SwaggerOperation(Summary = "Confirm AI resources", Description = "Confirms AI resources for a course by ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, "AI resources confirmed successfully.", typeof(ApiResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.", typeof(ApiResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
        public async Task<ActionResult<ApiResponse>> Update(int id, ConfirmAIResourcesCommand command)
        {
            command.CourseId = id;
            var result = await _mediator.Send(command);
            return HandleResponse(this, result);
        }

        [HttpDelete("{resourceId}/delete")]
        [Authorize(Roles = UserRoles.Instructor)]
        [SwaggerOperation(Summary = "Delete enrollment", Description = "Removes a student enrollment from a course.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Enrollment deleted successfully.", typeof(ApiResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Course or student not found.", typeof(ApiResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
        public async Task<ActionResult<ApiResponse>> DeleteEnrollment(int id, Guid resourceId)
        {

            var result = await _mediator.Send(new DeleteAIResourcesCommand { CourseId = id, AiResourceId = resourceId });
            return HandleResponse(this, result);
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Instructor)]
        [SwaggerOperation(Summary = "Get AI resources", Description = "Lists AI resources for a course.")]
        [SwaggerResponse(StatusCodes.Status200OK, "AI resources retrieved successfully.", typeof(ApiResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.", typeof(ApiResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
        public async Task<ActionResult<ApiResponse>> GetAIResources(int id)
        {
            var result = await _mediator.Send(new GetAIResourcesCommand { CourseId = id });
            return HandleResponse(this, result);
        }

        [HttpPost("process")]
        [Authorize(Roles = UserRoles.Instructor)]
        [SwaggerOperation(Summary = "Process AI resources", Description = "Processes AI resources for a course.")]
        [SwaggerResponse(StatusCodes.Status200OK, "AI resources processed successfully.", typeof(ApiResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.", typeof(ApiResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
        public async Task<ActionResult<ApiResponse>> ProcessAiResources(int id, ProcessAiResourcesRequest request)
        {
            var result = await _mediator.Send(new AIServiceProcessDocsCommand (id, request.AiResourceIds));
            return HandleResponse(this, result);
        }
    }
}
