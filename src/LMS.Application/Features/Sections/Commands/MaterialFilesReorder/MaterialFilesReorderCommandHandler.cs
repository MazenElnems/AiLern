using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Sections.Commands.MaterialFilesReorder;

public class MaterialFilesReorderCommandHandler : IRequestHandler<MaterialFilesReorderCommand, Result>
{
    private readonly IPermissionService _permissionService;
    private readonly IUnitOfWork _unitOfWork;

    public MaterialFilesReorderCommandHandler(IPermissionService permissionService, IUnitOfWork unitOfWork)
    {
        _permissionService = permissionService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(MaterialFilesReorderCommand request, CancellationToken cancellationToken)
    {
        var sectionResult = await _permissionService.AuthorizeInstructorAccessToSectionAsync(request.sectionId);
        if (!sectionResult.IsSuccess) return Result.Failure(sectionResult.Error!);

        var sectionWithFiles = await _unitOfWork.Sections.GetAsync(sec => sec.Id == request.sectionId,
            includeProperties: [nameof(Section.MaterialFiles)]);

        var files = sectionWithFiles!.MaterialFiles;

        if (!files.Any())
            return DomainErrors.Section.Empty;

        if (!request.OrderedFilesIds.All(id => files.Any(file => file.Id == id)))
            return DomainErrors.Common.BusinessRule("Invalid Files Reorder Request", "One or more files in the reorder request do not belong to this section.");

        for (int i = 0; i < request.OrderedFilesIds.Count; i++)
        {
            var file = files.First(f => f.Id == request.OrderedFilesIds[i]);
            file.OrderIndex = i + 1;
        }

        await _unitOfWork.CommitAsync();

        return Result.Success("Section files have been reordered successfully.");
    }
}
