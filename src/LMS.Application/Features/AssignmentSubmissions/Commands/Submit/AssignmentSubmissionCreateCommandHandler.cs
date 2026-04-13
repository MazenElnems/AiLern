using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.Jobs;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.AssignmentSubmissions.Shared.DTO;
using LMS.Domain.Entities.Assignments;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.AssignmentSubmissions.Commands.Submit;

public class AssignmentSubmissionCreateCommandHandler : IRequestHandler<AssignmentSubmissionCreateCommand, Result<AssignmentSubmissionDto>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IWasabiService _wasabiService;
    private readonly IBackgroundJobService _backgroundService;

    public AssignmentSubmissionCreateCommandHandler(IUserContext userContext, IUnitOfWork unitOfWork, IMapper mapper, IWasabiService wasabiService, IBackgroundJobService backgroundService)
    {
        _userContext = userContext;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _wasabiService = wasabiService;
        _backgroundService = backgroundService;
    }

    public async Task<Result<AssignmentSubmissionDto>> Handle(AssignmentSubmissionCreateCommand request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();

        var assignment = await _unitOfWork.Assignments.GetAsync(a => a.Id == request.AssignmentId,
            includeProperties: [nameof(Course)]);

        if (assignment == null)
            return DomainErrors.Assignment.NotFound(request.AssignmentId);

        if(await _unitOfWork.AssignmentSubmissions.AnyAsync(s => s.StudentId == user.Id && s.AssignmentId == assignment.Id))
            return DomainErrors.AssignmentSubmission.AlreadySubmitted;

        var course = assignment.Course;

        if(!await _unitOfWork.Enrollments.IsEnrolledAsync(course.Id, user.Id))
            return DomainErrors.Course.NotEnrolled;

        var submission = new AssignmentSubmission
        {
            SubmissionDate = DateTime.UtcNow,
        };

        var isLate = submission.SubmissionDate > assignment.DueDate;

        if(isLate)
            submission.IsLate = true;

        if(submission.IsLate && !assignment.AllowLateSubmission)
            return DomainErrors.AssignmentSubmission.LateNotAllowed;

        submission.StudentId = user.Id;
        assignment.Submissions.Add(submission);

        List<string> fileUrls = new();
        List<string> keys = new();
        foreach (var file in request.FileMetaData)
        {
            var key = $"courses/{course.Name}/assignments/{assignment.Id}/submissions/{submission.Id}/{file.FileName}";
            var url = await _wasabiService.GeneratePresignedUploadUrlAsync(key, file.ContentType, 2);
            fileUrls.Add(url);

            submission.Files.Add(new AssignmentSubmissionFile
            {
                FileName = file.FileName,
                StoragePath = key,
                FileType = file.ContentType,
                UploadStatus = UploadStatus.Pending
            });
            keys.Add(key);
        }

        await _unitOfWork.CommitAsync();
        _backgroundService.Schedule<IConfirmUploadedFilesJob>(
            job => job.ExecuteAsync(keys),
            TimeSpan.FromMinutes(2));

        var dto = _mapper.Map<AssignmentSubmissionDto>(submission);
        dto.UploadFilesUrls = fileUrls;

        return dto;
    }
}
