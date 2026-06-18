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

namespace LMS.Application.Features.Assignments.Commands.UpdateAssignment;

public class AssignmentUpdateCommandHandler : IRequestHandler<AssignmentUpdateCommand, Result<AssignmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IWasabiService _wasabiService;
    private readonly INotificationService _notificationService;
    private readonly IBackgroundJobService _backgroundService;

    public AssignmentUpdateCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper, IWasabiService wasabiService, INotificationService notificationService, IBackgroundJobService backgroundService)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _mapper = mapper;
        _wasabiService = wasabiService;
        _notificationService = notificationService;
        _backgroundService = backgroundService;
    }

    public async Task<Result<AssignmentDto>> Handle(AssignmentUpdateCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;

        var assignment = await _unitOfWork.Assignments.GetAsync(a => a.Id == request.Id,
            includeProperties: [nameof(Assignment.Course), nameof(Assignment.Submissions)]);

        if (assignment == null)
            return DomainErrors.Assignment.NotFound(request.Id);

        if (assignment.Course.InstructorId != userId)
            return DomainErrors.Course.NotOwned;

        var assignmentDueDateChanged = assignment.DueDate != request.DueDate;

        if (assignmentDueDateChanged && request.DueDate < DateTime.UtcNow)
            return DomainErrors.Assignment.InValidDueDate;

        var previousDueDate = assignment.DueDate;
        var previousIsPublished = assignment.IsPublished;

        assignment.Title = request.Title;
        assignment.Instructions = request.Instructions;
        assignment.DueDate = request.DueDate;
        assignment.AllowLateSubmission = request.AllowLateSubmission;
        assignment.IsPublished = request.IsPublished;

        if (assignment.IsPublished && request.DueDate > previousDueDate)
        {
            foreach (var submission in assignment.Submissions)
            {
                if (submission.IsLate && submission.SubmissionDate <= request.DueDate)
                    submission.IsLate = false;
            }
        }

        var dto = _mapper.Map<AssignmentDto>(assignment);
        List<string> keys = new();
        if (request.UploadedFileMetaData is not null)
        {
            foreach (var file in request.UploadedFileMetaData)
            {
                var key = $"courses/{assignment.Course.Name}/assignments/{assignment.Id}/{Guid.NewGuid()}_{file.FileName}";
                var url = await _wasabiService.GeneratePresignedUploadUrlAsync(key, file.ContentType, 2);
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



        await _unitOfWork.CommitAsync();

        if (!previousIsPublished && assignment.IsPublished)
        {
            await _notificationService.NotifyAsync(assignment.CourseId,
                $"{assignment.Course.Name}: New Assignment",
                $"\"{assignment.Title}\" is available. Due by {assignment.DueDate:MMM dd}.",
                NotificationType.NewAssignmentAdded,
                $"assignments/{assignment.Id}",
                "View Assignment");
        }
        return Result<AssignmentDto>.Success(dto, "Assignment updated successfully.");
    }
}
