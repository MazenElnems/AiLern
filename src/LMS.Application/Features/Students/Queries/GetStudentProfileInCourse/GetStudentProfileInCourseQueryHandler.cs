using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.AssignmentSubmissions.Shared.DTO;
using LMS.Application.Features.Students.Shared.DTO;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Students.Queries.GetStudentProfileInCourse;

public class GetStudentProfileInCourseQueryHandler : IRequestHandler<GetStudentProfileInCourseQuery, Result<GetStudentProfileInCourseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IBunnyUrlSigner _bunnyUrlSigner;

    public GetStudentProfileInCourseQueryHandler(IUnitOfWork unitOfWork, IUserContext userContext, IBunnyUrlSigner bunnyUrlSigner)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _bunnyUrlSigner = bunnyUrlSigner;
    }

    public async Task<Result<GetStudentProfileInCourseDto>> Handle(GetStudentProfileInCourseQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;
        var course = await _unitOfWork.Courses.GetAsync(c => c.Id == request.CourseId);
        if (course == null)
        {
            return DomainErrors.Course.NotFound(request.CourseId);
        }
        if (course.InstructorId != userId)
        {
            return DomainErrors.Course.NotOwned;
        }
        var isEnrolled = await _unitOfWork.Enrollments.IsEnrolledAsync(request.CourseId, request.StudentId);
        if (!isEnrolled)
        {
            return DomainErrors.Course.NotEnrolled;
        }
        var quizzes = await _unitOfWork.Quizzes.Query
            .AsNoTracking()
            .Where(q => q.CourseId == request.CourseId)
            .Select(q => new QuizProfileDto
            {
                QuizId = q.Id,
                QuizName = q.Title,
                Attempts = q.Attempts.Where(s => s.StudentId == request.StudentId).Select(s => new AttemptProfileDto
                {
                    AttemptId = s.Id,
                    AttemptNumber = s.AttemptNumber,
                    Score = s.Score,
                    SubmittedAt = s.SubmittedAt
                }).ToList()
            }).ToListAsync(cancellationToken);
        var attempts = quizzes
            .SelectMany(q => q.Attempts)
            .Where(a => a.Score.HasValue);

        var averageQuizzesScore = attempts.Any()
            ? attempts.Average(a => a.Score!.Value)
            : 0;
        var assignments = await _unitOfWork.Assignments.Query
            .AsNoTracking()
            .Where(a => a.CourseId == request.CourseId)
            .Select(a => new
            {
                Assignment = a,
                Submission = a.Submissions
        .Where(s => s.StudentId == request.StudentId)
        .FirstOrDefault()
            })
        .Select(x => new SubmissionProfileDto
        {
            AssignmentId = x.Assignment.Id,
            AssignmentName = x.Assignment.Title,
            SubmissionId = x.Submission != null ? x.Submission.Id : 0,
            SubmissionFiles = x.Submission != null
                ? x.Submission.Files.Select(sf => new MySubmissionFilesDto
                {
                    Id = sf.Id,
                    FileName = sf.FileName,
                    FileUrl = _bunnyUrlSigner.GenerateSignedUrl(
                        sf.StoragePath,
                        TimeSpan.FromMinutes(15)),
                    FileType = sf.FileType
                }).ToList()
                : new List<MySubmissionFilesDto>(),
            SubmissionFeedback = x.Submission != null ? x.Submission.Feedback : null
        }).ToListAsync(cancellationToken);
        var dto = new GetStudentProfileInCourseDto
        {
            Quizzes = quizzes,
            AverageQuizzesScore = averageQuizzesScore,
            Assignments = assignments
        };
        return dto;
    }
}
