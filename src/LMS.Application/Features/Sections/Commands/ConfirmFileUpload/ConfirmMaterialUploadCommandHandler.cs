using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Sections.Commands.ConfirmFileUpload;

public class ConfirmMaterialUploadCommandHandler : IRequestHandler<ConfirmMaterialUploadCommand, Result>
{
    private readonly IPermissionService _permissionService;
    private readonly IWasabiService _wasabiService;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmMaterialUploadCommandHandler(IPermissionService permissionService, IWasabiService wasabiService, IUnitOfWork unitOfWork)
    {
        _permissionService = permissionService;
        _wasabiService = wasabiService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ConfirmMaterialUploadCommand request, CancellationToken cancellationToken)
    {
        var sectionResult = await _permissionService.AuthorizeInstructorAccessToSectionAsync(request.SectionId);
        if (!sectionResult.IsSuccess) return Result.Failure(sectionResult.Error!);

        var sectionWithFiles = await _unitOfWork.Sections.GetAsync(sec => sec.Id == request.SectionId,
            includeProperties: [nameof(Section.MaterialFiles)]);

        foreach (var file in sectionWithFiles!.MaterialFiles)
        {
            var exist = await _wasabiService.FileExists(file.StoragePath);

            if (!exist)
                return DomainErrors.Storage.FileMissing;

            file.UploadStatus = UploadStatus.Completed;
        }

        await _unitOfWork.CommitAsync();
        return Result.Success();
    }
}
