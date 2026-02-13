using LMS.API.Common.Responses;
using LMS.API.Controllers.Common;
using LMS.Application.Features.Sections.Commands.ConfirmFileUpload;
using LMS.Application.Features.Sections.Commands.CreateSection;
using LMS.Application.Features.Sections.Commands.DeleteMaterialFile;
using LMS.Application.Features.Sections.Commands.DeleteSection;
using LMS.Application.Features.Sections.Commands.MaterialFilesReorder;
using LMS.Application.Features.Sections.Commands.RequestPreSignedUrl;
using LMS.Application.Features.Sections.Commands.UpdateSection;
using LMS.Application.Features.Sections.Queries.GetMaterialFile;
using LMS.Application.Features.Sections.Queries.GetSection;
using LMS.Application.Features.Sections.Queries.GetSectionFiles;
using LMS.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

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
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpPost("{id}/presigned-url")]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<ActionResult<ApiResponse>> RequestMaterialUpload(Guid id, RequestMaterialPresignedUrlCommand command)
    {
        command.SectionId = id;
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpPost("{id}/confirm-upload")]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<ActionResult<ApiResponse>> ConfirmMaterialUpload(Guid id)
    {
        var result = await _mediator.Send(new ConfirmMaterialUploadCommand { SectionId = id });
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

    [HttpDelete("{id}/files/{fileId}")]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<ActionResult<ApiResponse>> DeleteMaterialFile(Guid id,Guid fileId)
    {
        var result = await _mediator.Send(new DeleteMaterialFileCommand(id, fileId));
        return HandleResponse(this, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<ActionResult<ApiResponse>> DeleteSection(Guid id)
    {
        var result = await _mediator.Send(new SectionDeleteCommand(id));
        return HandleResponse(this, result);
    }

    [HttpGet("{Id}/files")]
    [Authorize(Roles = $"{UserRoles.Instructor},{UserRoles.Student}")]
    public async Task<ActionResult<ApiResponse>> GetSectionFiles(Guid Id)
    {
        var result = await _mediator.Send(new GetSectionFilesQuery(Id));
        return HandleResponse(this, result);
    }

    [HttpGet("courses/{courseId}/sections")]
    [Authorize(Roles = $"{UserRoles.Instructor},{UserRoles.Student}")]
    public async Task<ActionResult<ApiResponse>> GetCourseSections(int courseId)
    {
        var result = await _mediator.Send(new GetCourseSectionsQuery(courseId));
        return HandleResponse(this, result);
    }

    [HttpGet("{id}/files/{fileId}")]
    [Authorize(Roles = $"{UserRoles.Instructor},{UserRoles.Student}")]
    public async Task<ActionResult<ApiResponse>> GetMaterialFile(Guid id, Guid fileId)
    {
        var result = await _mediator.Send(new GetMaterialFileQuery { Id = id, FileId = fileId });
        return HandleResponse(this, result);
    }
    

    [HttpPut("{id}/files/reorder")]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<ActionResult<ApiResponse>> ReorderMaterialFiles(Guid id,MaterialFilesReorderCommand command)
    {
        command.sectionId = id;
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpGet("{sectionId}")]
    [Authorize(Roles = $"{UserRoles.Instructor},{UserRoles.Student}")]
    public async Task<ActionResult<ApiResponse>> GetSection(Guid sectionId)
    {
        var result = await _mediator.Send(new GetSectionQuery(sectionId));
        return HandleResponse(this, result);
    }
}
