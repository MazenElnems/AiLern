using LMS.API.Common.Responses;
using LMS.API.Controllers.Common;
using LMS.Application.Features.Assignments.Commands.DeleteAssignment;
using LMS.Application.Features.Assignments.Commands.UpdateAssignment;
using LMS.Application.Features.Assignments.Queries.GetAssignment;
using LMS.Application.Features.Sections.Commands.CreateSection;
using LMS.Application.Features.Sections.Commands.DeleteSection;
using LMS.Application.Features.Sections.Commands.UpdateSection;
using LMS.Application.Features.Sections.Queries.GetSection;
using LMS.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionsController : ApiBaseController
    {
        private readonly IMediator _mediator;


        public SectionsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        [Authorize(Roles =UserRoles.Instructor)]
        public  async Task<ActionResult<ApiResponse>> Create(SectionCreateCommand command)
        {
            var result  = await _mediator.Send(command);
            return HandleResponse(this, result);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = UserRoles.Instructor)]
        public async Task<ActionResult<ApiResponse>> Update(Guid id, SectionUpdateCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return HandleResponse(this, result);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Instructor)]
        public async Task<ActionResult<ApiResponse>> Delete(Guid id)
        {
            var result = await _mediator.Send(new SectionDeleteCommand(id));
            return HandleResponse(this, result);
        }

        [HttpGet("courses/{courseId}/sections")]
        [Authorize(Roles = $"{UserRoles.Instructor},{UserRoles.Student}")]
        public async Task<ActionResult<ApiResponse>> GetCourseSections(int courseId)
        {
            var result = await _mediator.Send(new GetCourseSectionsQuery(courseId));
            return HandleResponse(this, result);
        }

        [HttpGet("/{sectionId}")]
        [Authorize(Roles = $"{UserRoles.Instructor},{UserRoles.Student}")]
        public async Task<ActionResult<ApiResponse>> GetSection(Guid sectionId)
        {
            var result = await _mediator.Send(new GetSectionQuery(sectionId));
            return HandleResponse(this, result);
        }
    }
}
