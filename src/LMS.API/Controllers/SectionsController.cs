using LMS.API.Common.Responses;
using LMS.API.Controllers.Common;
using LMS.Application.Common.Results;
using LMS.Application.Features.Sections.Commands.ConfirmFileUpload;
using LMS.Application.Features.Sections.Commands.DeleteMaterialFile;
using LMS.Application.Features.Sections.Commands.MaterialFilesReorder;
using LMS.Application.Features.Sections.Commands.RequestPreSignedUrl;
using LMS.Application.Features.Sections.Queries.GetMaterialFile;
using LMS.Application.Features.Sections.Queries.GetSectionFiles;
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
using System.Threading.Tasks;

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

        [HttpPost("{id}/presigned-url")]
        [Authorize(Roles = UserRoles.Instructor)]
        public async Task<ActionResult<ApiResponse>> RequestMaterialUpload(Guid id, RequestMaterialPresignedUrlCommand command)
        {
            command.SectionId = id;
            var result = await _mediator.Send(command);
            var result  = await _mediator.Send(command);
            return HandleResponse(this, result);
        }

        [HttpPost("{id}/confirm-upload")]
        [HttpPut("{id}")]
        [Authorize(Roles = UserRoles.Instructor)]
        public async Task<ActionResult<ApiResponse>> ConfirmMaterialUpload(Guid id)
        public async Task<ActionResult<ApiResponse>> Update(Guid id, SectionUpdateCommand command)
        {
            var result = await _mediator.Send(new ConfirmMaterialUploadCommand { SectionId = id });
            command.Id = id;
            var result = await _mediator.Send(command);
            return HandleResponse(this, result);
        }

        [HttpDelete("{id}/files/{fileId}")]
        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Instructor)]
        public async Task<ActionResult<ApiResponse>> MaterialFile(Guid id,Guid fileId)
        public async Task<ActionResult<ApiResponse>> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteMaterialFileCommand( id,fileId ));
            var result = await _mediator.Send(new SectionDeleteCommand(id));
            return HandleResponse(this, result);
        }

        [HttpGet("{Id}/files")]
        [HttpGet("courses/{courseId}/sections")]
        [Authorize(Roles = $"{UserRoles.Instructor},{UserRoles.Student}")]
        public async Task<ActionResult<ApiResponse>> SectionFiles(Guid Id)
        public async Task<ActionResult<ApiResponse>> GetCourseSections(int courseId)
        {
            var result = await _mediator.Send(new GetSectionFilesQuery(Id));
            var result = await _mediator.Send(new GetCourseSectionsQuery(courseId));
            return HandleResponse(this, result);
        }

        [HttpGet("{id}/files/{fileId}")]
        [HttpGet("/{sectionId}")]
        [Authorize(Roles = $"{UserRoles.Instructor},{UserRoles.Student}")]
        public async Task<ActionResult<ApiResponse>> GetMaterialFile(Guid id, Guid fileId)
        {
            var result = await _mediator.Send(new GetMaterialFileQuery { Id = id, FileId = fileId });
            return HandleResponse(this, result);
        }

        [HttpPut("{id}/files/reorder")]
        [Authorize(Roles = UserRoles.Instructor)]
        public async Task<ActionResult<ApiResponse>> MaterialFilesReorder(Guid id,MaterialFilesReorderCommand command)
        public async Task<ActionResult<ApiResponse>> GetSection(Guid sectionId)
        {
            command.sectionId = id;
            var result = await  _mediator.Send(command);
            var result = await _mediator.Send(new GetSectionQuery(sectionId));
            return HandleResponse(this, result);
        }

    }
}
