using LMS.Application.Common.Results;
using LMS.Application.CurrentUser;
using LMS.Domain.Common.Errors;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Sections.Commands.DeleteMaterialFile;

internal class DeleteMaterialFileCommandHandler : IRequestHandler<DeleteMaterialFileCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IWasabiService _wasabiService;

    public DeleteMaterialFileCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IWasabiService wasabiService)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _wasabiService = wasabiService;
    }

    public async Task<Result> Handle(DeleteMaterialFileCommand request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();

        var section = await _unitOfWork.Sections.GetAsync(sec => sec.Id == request.SectionId,
            includeProperties: [nameof(Section.Course), nameof(Section.MaterialFiles)]);

        if(section == null)
            return DomainErrors.Section.NotFound(request.SectionId);

        if (section.Course.InstructorId != user.Id)
            return DomainErrors.Common.Forbidden("You do not have permission to delete this section file.");

        var file = section.MaterialFiles.FirstOrDefault(f => f.Id == request.FileId);
        if (file == null)
            return DomainErrors.MaterialFile.NotFound(request.FileId);

        var filePath = file.StoragePath;
        try
        {
            await _wasabiService.DeleteFileAsync(filePath);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to delete file from storage.", ex);
        }

        _unitOfWork.MaterialFiles.DeleteFile(file);
        await _unitOfWork.CommitAsync();

        return Result.Success();
    }
}
