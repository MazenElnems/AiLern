using LMS.Application.Common.Results;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.Courses.Commands.DeleteAIResources;

public class DeleteAIResourcesCommandHandler : IRequestHandler<DeleteAIResourcesCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _user;
    private readonly IWasabiService _wasabi;

    public DeleteAIResourcesCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IWasabiService wasabi)
    {
        _unitOfWork = unitOfWork;
        _user = userContext;
        _wasabi = wasabi;
    }

    public async Task<Result> Handle(DeleteAIResourcesCommand request, CancellationToken cancellationToken)
    {
        var userId = _user.GetCurrentUser().Id;
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course == null)
        {
            return DomainErrors.Course.NotFound(request.CourseId);
        }
        if (course.InstructorId != userId)
        {
            return DomainErrors.Course.NotOwned;
        }
        var aiResource = await _unitOfWork.AIResources.GetByIdAsync(request.AiResourceId);
        if (aiResource == null || aiResource.CourseId != request.CourseId)
        {
            return DomainErrors.AiResource.NotFound(request.AiResourceId);
        }
        _unitOfWork.AIResources.Delete(aiResource);
        var isExist = await _wasabi.FileExists(aiResource.StoragePath);
        if (!isExist)
        {
            return DomainErrors.Common.BusinessRule("FileNotFound","The file does not exist in storage.");
        }
        try
        {
            await _wasabi.DeleteFileAsync(aiResource.StoragePath, cancellationToken, true);
            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            throw ;
        }
        return Result.Success("Deleted Success");


    }
}
