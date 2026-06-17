using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.AssignmentSubmissions.Shared.DTO;
using LMS.Application.Features.Students.Shared.DTO;
using LMS.Domain.Entities.Courses;
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

        var course = await _unitOfWork.Courses.GetAsync(c => c.Id == request.CourseId,
            includeProperties: [nameof(Course.Sections)]);

        if (course == null)
            return DomainErrors.Course.NotFound(request.CourseId);
        
        if (course.InstructorId != userId)
            return DomainErrors.Course.NotOwned;
        
        var isEnrolled = await _unitOfWork.Enrollments.IsEnrolledAsync(request.CourseId, request.StudentId);
        
        if (!isEnrolled)
            return DomainErrors.Course.NotEnrolled;
        
        var quizzes = await _unitOfWork.Quizzes.Query
            .AsNoTracking()
            .Where(q => q.CourseId == request.CourseId)
            .Include(q => q.Questions)
            .Include(q => q.Attempts)
                .ThenInclude(q => q.Answers)
            .AsSplitQuery()
            .Select(q => new QuizProfileDto
            {
                QuizId = q.Id,
                QuizName = q.Title,
                TotalPoints = q.TotalPoints,
                Attempts = q.Attempts.Where(s => s.StudentId == request.StudentId).Select(s => new AttemptProfileDto
                {
                    AttemptId = s.Id,
                    AttemptNumber = s.AttemptNumber,
                    Score = s.Score,
                    SubmittedAt = s.SubmittedAt
                }).ToList(),
            }).ToListAsync(cancellationToken);

        var studentProgressPerQuiz = quizzes
            .Select(q => new
            {
                QuizProgress = q.Attempts.Any() ? q.Attempts.Average(a => a.Score.HasValue ? a.Score.Value : 0) / q.TotalPoints : default(double?)
            });

        var averageQuizzesScore = studentProgressPerQuiz.Any(s => s.QuizProgress.HasValue) ? studentProgressPerQuiz
            .Where(s => s.QuizProgress.HasValue)
            .Average(a => a.QuizProgress!.Value) * 100 : 0;

        var assignments = await _unitOfWork.Assignments.Query
            .AsNoTracking()
            .Where(a => a.CourseId == request.CourseId)
            .Select(a => new
            {
                Assignment = a,
                Submission = a.Submissions.FirstOrDefault(s => s.StudentId == request.StudentId)
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

        var completedSections = await _unitOfWork.SectionProgress.Query.AsNoTracking()
            .Where(p => p.IsCompleted && p.StudentId == request.StudentId && p.Section.CourseId == request.CourseId)
            .CountAsync(cancellationToken);

        var progress = course.Sections.Count() == 0
                ? 0
                : (int)(completedSections * 100M
                    /
                  course.Sections.Count);

        var dto = new GetStudentProfileInCourseDto
        {
            Quizzes = quizzes,
            AverageQuizzesScore = (double) Math.Round((decimal)averageQuizzesScore),
            Assignments = assignments,
            Progress = progress
        };
        return dto;
    }
}
