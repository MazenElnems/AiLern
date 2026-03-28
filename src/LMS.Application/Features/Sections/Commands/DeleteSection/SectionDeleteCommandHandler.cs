using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Sections.Commands.DeleteSection;

public class SectionDeleteCommandHandler : IRequestHandler<SectionDeleteCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;
    private readonly IWasabiService _wasabiService;
    private readonly ILogger<SectionDeleteCommandHandler> _logger;

    public SectionDeleteCommandHandler(IPermissionService permissionService, IUnitOfWork unitOfWork, IWasabiService wasabiService, ILogger<SectionDeleteCommandHandler> logger)
    {
        _permissionService = permissionService;
        _unitOfWork = unitOfWork;
        _wasabiService = wasabiService;
        _logger = logger;
    }

    public async Task<Result> Handle(SectionDeleteCommand request, CancellationToken cancellationToken)
    {
        var sectionResult = await _permissionService.AuthorizeInstructorAccessToSectionAsync(request.Id);
        if (!sectionResult.IsSuccess) return Result.Failure(sectionResult.Error!);
        var section = sectionResult.Value!;

        var sectionWithFiles = await _unitOfWork.Sections.GetAsync(s => s.Id == request.Id,
            [nameof(Section.MaterialFiles)]);

        var filePaths = sectionWithFiles!.MaterialFiles.Select(f => f.StoragePath);

        _unitOfWork.Sections.Delete(section);
        await _unitOfWork.CommitAsync();

        try
        {
            foreach (var filePath in filePaths)
            {
                await _wasabiService.DeleteFileAsync(filePath, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting section files from Wasabi.");
        }

        return Result.Success();
    }
}
