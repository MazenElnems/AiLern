using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Sections.Commands.DeleteMaterialFile;

internal class DeleteMaterialFileCommandHandler : IRequestHandler<DeleteMaterialFileCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;
    private readonly IWasabiService _wasabiService;

    public DeleteMaterialFileCommandHandler(IUnitOfWork unitOfWork, IPermissionService permissionService, IWasabiService wasabiService)
    {
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
        _wasabiService = wasabiService;
    }

    public async Task<Result> Handle(DeleteMaterialFileCommand request, CancellationToken cancellationToken)
    {
        var sectionResult = await _permissionService.AuthorizeInstructorAccessToSectionAsync(request.SectionId);
        if (!sectionResult.IsSuccess) return Result.Failure(sectionResult.Error!);
        var section = sectionResult.Value!;

        var sectionWithFiles = await _unitOfWork.Sections.GetAsync(sec => sec.Id == request.SectionId,
            includeProperties: [nameof(Section.MaterialFiles)]);

        var file = sectionWithFiles!.MaterialFiles.FirstOrDefault(f => f.Id == request.FileId);

        if (file == null)
            return DomainErrors.MaterialFile.NotFound(request.FileId);

        var filePath = file.StoragePath;
        try
        {
            await _wasabiService.DeleteFileAsync(filePath, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to delete file from storage.", ex);
        }

        section.RemoveFile(file);
        await _unitOfWork.CommitAsync();

        return Result.Success();
    }
}
