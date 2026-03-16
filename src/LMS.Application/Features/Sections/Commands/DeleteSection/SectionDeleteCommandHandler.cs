using LMS.Application.Common.Results;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Sections.Commands.DeleteSection;

public class SectionDeleteCommandHandler : IRequestHandler<SectionDeleteCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IWasabiService _wasabiService;
    private readonly ILogger<SectionDeleteCommandHandler> _logger;


    public SectionDeleteCommandHandler(IUserContext userContext, IUnitOfWork unitOfWork, IWasabiService wasabiService, ILogger<SectionDeleteCommandHandler> logger)
    {
        _userContext = userContext;
        _unitOfWork = unitOfWork;
        _wasabiService = wasabiService;
        _logger = logger;
    }

    public async Task<Result> Handle(SectionDeleteCommand request, CancellationToken cancellationToken)
    {
        var section = await _unitOfWork.Sections.GetAsync(a => a.Id == request.Id,
            [nameof(Section.Course), nameof(Section.MaterialFiles)]);
        if (section == null)
        {
            return DomainErrors.Section.NotFound(request.Id);
        }
        var userId = _userContext.GetCurrentUser().Id;
        if (section.Course.InstructorId != userId)
            return DomainErrors.Common.Forbidden("You do not have permission to delete this section.");

        var filePaths = section.MaterialFiles.Select(f => f.StoragePath);

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
