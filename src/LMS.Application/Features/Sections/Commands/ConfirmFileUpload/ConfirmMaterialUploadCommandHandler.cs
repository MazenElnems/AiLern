using LMS.Application.Common.Results;
using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.Sections.Commands.ConfirmFileUpload;

public class ConfirmMaterialUploadCommandHandler : IRequestHandler<ConfirmMaterialUploadCommand, Result>
{
    private readonly IUserContext _userContext;
    private readonly IWasabiService _wasabiService;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmMaterialUploadCommandHandler(IUserContext userContext, IWasabiService wasabiService, IUnitOfWork unitOfWork)
    {
        _userContext = userContext;
        _wasabiService = wasabiService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ConfirmMaterialUploadCommand request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();

        var section = await _unitOfWork.Sections.GetAsync(sec => sec.Id == request.SectionId,
            includeProperties: [nameof(Section.Course), nameof(Section.MaterialFiles)]);

        if (section == null)
            return DomainErrors.Section.NotFound(request.SectionId);
        var course = section.Course;

        if(course.InstructorId != user.Id)
            return DomainErrors.Common.Forbidden("You do not have permission to request pre-signed URLs for this section.");

        foreach(var file in section.MaterialFiles)
        {
            var exist = await _wasabiService.FileExists(file.StoragePath);

            if(!exist)
                return DomainErrors.Storage.FileMissing;

            file.UploadStatus = UploadStatus.Completed;
        }

        await _unitOfWork.CommitAsync();
        return Result.Success();
    }
}
