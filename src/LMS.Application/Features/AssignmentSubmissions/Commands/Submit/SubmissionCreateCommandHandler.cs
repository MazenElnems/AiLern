using AutoMapper;
using LMS.Application.CurrentUser;
using LMS.Domain.Constants;
using LMS.Application.Common.Results.Generic;
using LMS.Domain.Common.Errors;
using LMS.Domain.DTOs.Assignments;
using LMS.Domain.DTOs.Submission;
using LMS.Domain.Entities;
using LMS.Domain.Common.Enums;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace LMS.Application.Features.AssignmentSubmissions.Commands.Submit;

public class SubmissionCreateCommandHandler : IRequestHandler<SubmissionCreateCommand, Result<SubmissionDto>>
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

    public async Task<Result<SubmissionDto>> Handle(SubmissionCreateCommand request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();
        var assignment = await _unitOfWork.Assignments.GetAsync(a => a.Id == request.AssignmentId, [nameof(Course)]);
        if (assignment == null) 
        {
            return Result<SubmissionDto>.Failure(DomainErrors.Assignment.NotFound(request.AssignmentId));
        }
        var course = assignment.Course;
        var isEnrolled = await _unitOfWork.Enrollments.IsEnrolledAsync(course.Id,user.Id);


        if (!isEnrolled)
        {
            return Result<SubmissionDto>.Failure(DomainErrors.Submission.NotEnrolled);
        }
        var submission = _mapper.Map<AssignmentSubmission>(request);
        submission.SubmissionDate = DateTime.UtcNow;
        var islate = assignment.DueDate < submission.SubmissionDate;
        if (islate && assignment.AllowLateSubmission == false)
        {
            return Result<SubmissionDto>.Failure(DomainErrors.Submission.LateNotAllowed);
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

        return Result<SubmissionDto>.Success(_mapper.Map<SubmissionDto>(submission));




    }
}
