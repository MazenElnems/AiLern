using AutoMapper;
using LMS.Application.CurrentUser;
using LMS.Application.Common.Results.Generic;
using LMS.Domain.Common.Errors;
using LMS.Domain.Repositories;
using MediatR;
using LMS.Domain.Common.Enums;
using LMS.Domain.Entities.Courses;
using LMS.Application.DTOs.AssignmentSubmissions;
using LMS.Domain.Entities.Assignments;

namespace LMS.Application.Features.AssignmentSubmissions.Commands.Submit;

public class AssignmentSubmissionCreateCommandHandler : IRequestHandler<AssignmentSubmissionCreateCommand, Result<AssignmentSubmissionDto>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IWasabiService _wasabiService;

    public AssignmentSubmissionCreateCommandHandler(IUserContext userContext, IUnitOfWork unitOfWork, IMapper mapper, IWasabiService wasabiService)
    {
        _userContext = userContext;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _wasabiService = wasabiService;
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
        }

        await _unitOfWork.CommitAsync();
        
        var dto = _mapper.Map<AssignmentSubmissionDto>(submission);
        dto.UploadFilesUrls = fileUrls;

        return dto;
    }
}
