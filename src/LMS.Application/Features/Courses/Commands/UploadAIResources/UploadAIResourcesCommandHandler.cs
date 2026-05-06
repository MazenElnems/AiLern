using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Courses.Shared.DTO;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.Courses.Commands.UploadAIResources;

public class UploadAIResourcesCommandHandler : IRequestHandler<UploadAIResourcesCommand, Result<List<AIResourcesDto>>>
{
    private readonly IUserContext _user;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWasabiService _wasabiService;


    public UploadAIResourcesCommandHandler(IUserContext user, IUnitOfWork unitOfWork, IWasabiService wasabiService)
    {
        _user = user;
        _unitOfWork = unitOfWork;
        _wasabiService = wasabiService;
    }

    public async Task<Result<List<AIResourcesDto>>> Handle(UploadAIResourcesCommand request, CancellationToken cancellationToken)
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
        var aiResources = new List<AIResource>();
        foreach (var file in request.Files)
        {
            var aiResource = new AIResource
            {
                CourseId = request.CourseId,
                FileName = file.FileName,
                FileType = file.ContentType,
                FileSize = file.FileSize,
                StoragePath = $"courses/{course.Name.Replace(" ","_")}/AIResources/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}",
                UploadStatus = UploadStatus.Pending,
            };
            aiResources.Add(aiResource);
        }
        course.AIResources.AddRange(aiResources);
        await _unitOfWork.CommitAsync();

        var aiResourcesDtos = new List<AIResourcesDto>();
        foreach (var ar in aiResources)
        {
            var presignedUrl = await _wasabiService.GeneratePresignedUploadUrlAsync(ar.StoragePath, ar.FileType, 15, true);
            aiResourcesDtos.Add(new AIResourcesDto
            {
                FileId = ar.Id,
                PresignedUrl = presignedUrl
            });
        }
        return aiResourcesDtos;
    }
}
