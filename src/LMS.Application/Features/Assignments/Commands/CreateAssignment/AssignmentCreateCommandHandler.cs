using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.Jobs;
using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Assignments.Shared.DTO;
using LMS.Domain.Entities.Assignments;
using LMS.Domain.Entities.Notification;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Models.Notification;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LMS.Application.Features.Assignments.Commands.CreateAssignment;

public class AssignmentCreateCommandHandler : IRequestHandler<AssignmentCreateCommand, Result<AssignmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IWasabiService _wasabiService;
    private readonly IBackgroundJobService _backgroundService;
    private readonly INotificationService _notificationService;


    public AssignmentCreateCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper, IWasabiService wasabiService, IBackgroundJobService backgroundService, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _mapper = mapper;
        _wasabiService = wasabiService;
        _backgroundService = backgroundService;
        _notificationService = notificationService;
    }

    public async Task<Result<AssignmentDto>> Handle(AssignmentCreateCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);

        if(course == null)
            return DomainErrors.Course.NotFound(request.CourseId);

        if (course.InstructorId != userId)
            return DomainErrors.Course.NotOwned;

        var assignment = _mapper.Map<Assignment>(request);
        assignment.CourseId = request.CourseId;
        assignment.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Assignments.InsertAsync(assignment);
        await _unitOfWork.CommitAsync();

        var dto = _mapper.Map<AssignmentDto>(assignment);
        List<string> keys = new();
        if (request.UploadedFileMetaData is not null)
        {
            foreach (var file in request.UploadedFileMetaData)
            {
                var key = $"courses/{course.Id}/assignments/{assignment.Id}/{Guid.NewGuid()}.{file.FileName.Split('.').Last()}";
                var url = await _wasabiService.GeneratePresignedUploadUrlAsync(key, file.ContentType, 15);
                dto.PresingedFileUrls.Add(url);

                assignment.Files.Add(new AssignmentFile
                {
                    AssignmentId = assignment.Id,
                    FileName = file.FileName,
                    FileType = file.ContentType,
                    StoragePath = key,
                    UploadStatus = UploadStatus.Pending,
                });
                keys.Add(key);
            }
        }
        if (keys.Any())
        {
            _backgroundService.Schedule<IConfirmUploadedFilesJob>(
                job => job.ExecuteAsync(keys),
                TimeSpan.FromMinutes(2));
        }

        if (assignment.IsPublished)
        {
            await _notificationService.NotifyAsync(assignment.CourseId,
                $"{course.Name}: New Assignment",
                $"\"{assignment.Title}\" is available. Due by {assignment.DueDate:MMM dd}.",
                NotificationType.NewAssignmentAdded,
                $"assignments/{assignment.Id}", "View Assignment");
        }

        await _unitOfWork.CommitAsync();

        return Result<AssignmentDto>.Success(dto, "Assignment created successfully.");
    }
}
