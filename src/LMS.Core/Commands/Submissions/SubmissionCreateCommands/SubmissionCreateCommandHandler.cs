using AutoMapper;
using LMS.Core.CurrentUser;
using LMS.Domain.Constants;
using LMS.Domain.DTOs.Assignments;
using LMS.Domain.DTOs.Submission;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Domain.Exceptions;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace LMS.Core.Commands.Submissions.SubmissionCreateCommands;

public class SubmissionCreateCommandHandler : IRequestHandler<SubmissionCreateCommand, SubmissionDto>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public SubmissionCreateCommandHandler(IUserContext userContext, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _userContext = userContext;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SubmissionDto> Handle(SubmissionCreateCommand request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();
        var assignment = await _unitOfWork.Assignments.GetAsync(a => a.Id == request.AssignmentId, [nameof(Course)]);
        if (assignment == null) 
        {
            throw new ResourceNotFoundException(nameof(AssignmentSubmission), request.AssignmentId.ToString());
        }
        var course = assignment.Course;
        var isEnrolled = await _unitOfWork.Enrollments.IsEnrolledAsync(course.Id,user.Id);


        if (!isEnrolled)
        {
            throw new UnauthorizedAccessException("You are not enrolled in this course.");
        }
        var submission = _mapper.Map<AssignmentSubmission>(request);
        submission.SubmissionDate = DateTime.UtcNow;
        var islate = assignment.DueDate < submission.SubmissionDate;
        if (islate && assignment.AllowLateSubmission == false)
        {
            throw new InvalidOperationException("Late submission is not allowed for this assignment.");
        }
        else if (islate && assignment.AllowLateSubmission == true)
        {
            submission.IsLate = true;
        }
        else 
            submission.IsLate = false;
        submission.StudentId = user.Id;


        await _unitOfWork.Submissions.InsertAsync(submission);
        await _unitOfWork.CommitAsync();

        return _mapper.Map<SubmissionDto>(submission);




    }
}
