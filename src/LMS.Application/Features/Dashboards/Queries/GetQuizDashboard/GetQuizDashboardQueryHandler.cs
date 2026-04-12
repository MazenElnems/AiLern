using AutoMapper;
using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Dashboards.Shared.DTO;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Dashboards.Queries.GetQuizDashboard;

public class GetQuizDashboardQueryHandler : IRequestHandler<GetQuizDashboardQuery, Result<QuizDashboardDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;
    private readonly IMapper _mapper;
    private readonly IBackgroundJobService _backgroundJobService;

    public GetQuizDashboardQueryHandler(IUnitOfWork unitOfWork, IPermissionService permissionService, IMapper mapper, IBackgroundJobService backgroundJobService)
    public GetQuizDashboardQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
        _mapper = mapper;
        _backgroundJobService = backgroundJobService;
    }

    public async Task<Result<QuizDashboardDto>> Handle(GetQuizDashboardQuery request, CancellationToken cancellationToken)
    {
        var quizResult = await _permissionService.AuthorizeInstructorAccessToQuizAsync(request.quizId);
        if (!quizResult.IsSuccess)
            return Result<QuizDashboardDto>.Failure(quizResult.Error!);

        var quiz = quizResult.Value;
        var quiz = await _unitOfWork.Quizzes.GetByIdAsync(request.QuizId);

        var studentsInCourse = await _unitOfWork.Enrollments.Query
            .Where(e => e.CourseId == quiz!.CourseId)
            .CountAsync();

        var numberOfStudents = await _unitOfWork.Attempts.Query
            .Where(a => a.QuizId == request.quizId && a.Status == AttemptStatus.Reviewed)
            .Where(a => a.QuizId == request.QuizId && a.Status == AttemptStatus.Reviewed)
            .Select(a => a.StudentId)
            .Distinct()
            .CountAsync();

        var attemptScores = await _unitOfWork.AttemptAnswers.Query
            .Where(aa => aa.Attempt.QuizId == request.quizId && aa.Attempt.Status == AttemptStatus.Reviewed)
        var attemptScores = await _unitOfWork.Answers.Query
            .Where(aa => aa.Attempt.QuizId == request.QuizId && aa.Attempt.Status == AttemptStatus.Reviewed)
            .GroupBy(aa => new { aa.AttemptId, aa.Attempt.StudentId })
            .Select(g => new
            {
                g.Key.StudentId,
                Score = g.Sum(x => x.Mark)
            })
            .ToListAsync();

        var studentQuestionScores = await _unitOfWork.AttemptAnswers.Query
            .Where(a => a.Attempt.QuizId == request.quizId
        var studentQuestionScores = await _unitOfWork.Answers.Query
            .Where(a => a.Attempt.QuizId == request.QuizId
                        && a.Attempt.Status == AttemptStatus.Reviewed)
            .GroupBy(a => new { a.Attempt.StudentId, a.QuestionId, a.Question.Mark, a.Question.QuestionText })
            .Select(g => new
            {
                g.Key.StudentId,
                g.Key.QuestionId,
                g.Key.QuestionText,
                IsCorrect = g.Max(x => x.Mark) >= g.Key.Mark * 0.5
            })
            .ToListAsync();

        var questionStatistics = studentQuestionScores
            .GroupBy(x => new { x.QuestionId, x.QuestionText })
            .Select(g => new QuestionStatisticsDto
            {
                QuestionId = g.Key.QuestionId,
                QuestionText = g.Key.QuestionText,
                CorrectAnswersCount = g.Count(x => x.IsCorrect)
            })
            .ToList();

        var attemptDistributions = await _unitOfWork.Attempts.Query
            .Where(a => a.QuizId == request.quizId && a.Status == AttemptStatus.Reviewed)
            .Where(a => a.QuizId == request.QuizId && a.Status == AttemptStatus.Reviewed)
            .GroupBy(a => a.StudentId)
            .Select(g => g.Max(a => a.AttemptNumber))
            .GroupBy(x => x)
            .Select(g => new AttemptStatisticsDto
            {
                AttemptNumber = g.Key,
                StudentsCount = g.Count()
            })
            .ToListAsync();

        var times = await _unitOfWork.Attempts.Query
            .Where(a => a.QuizId == request.quizId && a.Status == AttemptStatus.Reviewed)
            .Where(a => a.QuizId == request.QuizId && a.Status == AttemptStatus.Reviewed)
            .Select(a => a.TimeSpent)
            .ToListAsync();

        var quizTotalPoints = await _unitOfWork.Quizzes.Query
            .Where(q => q.Id == request.quizId)
            .Where(q => q.Id == request.QuizId)
            .Select(q => q.Questions.Sum(a => a.Mark))
            .FirstOrDefaultAsync();

        var quarterOfQuiz = quiz!.AttemptTimeLimit * 0.25d;


        var studentScores = attemptScores
            .GroupBy(x => x.StudentId)
            .Select(g => new
            {
                Min = g.Min(x => x.Score),
                Avg = g.Average(x => x.Score),
                Max = g.Max(x => x.Score)
            })
            .ToList();

        var avaregeScore = new AverageScore
        {
            MinAverage = studentScores.Average(a => a.Min),
            AvgAverage = studentScores.Average(a => a.Avg),
            MaxAverage = studentScores.Average(a => a.Max)
        };


        var passDegree = quizTotalPoints * 0.5;

        var passesFalis = new Dictionary<string, QuizPassFailDto>
        {
            ["Min"] = new QuizPassFailDto
            {
                Passes = studentScores.Count(x => x.Min >= passDegree),
                Fails = studentScores.Count(x => x.Min < passDegree)
            },
            ["Avg"] = new QuizPassFailDto
            {
                Passes = studentScores.Count(x => x.Avg >= passDegree),
                Fails = studentScores.Count(x => x.Avg < passDegree)
            },
            ["Max"] = new QuizPassFailDto
            {
                Passes = studentScores.Count(x => x.Max >= passDegree),
                Fails = studentScores.Count(x => x.Max < passDegree)
            }
        };

        var submissionTimeDistribution = new List<SubmissionTimeBucketDto>();

        for (int i = 1; i <= 4; i++)
        {
            var start = Math.Round(quarterOfQuiz * (i - 1));
            var end = Math.Round(quarterOfQuiz * i);

            var count = times.Count(t => (t / 60) >= start && (t / 60) <= end);

            submissionTimeDistribution.Add(
                new SubmissionTimeBucketDto { 
                    BucketIndex = i,
                    Label = $"{start}-{end}min",
                    SubmissionsCount = count
                });
        }

        var dto = new QuizDashboardDto
        {
            StudentsInCourse = studentsInCourse,
            AverageScore = avaregeScore,
            PassesFalis = passesFalis,
            QuestionStatistics = questionStatistics,
            AttemptsDistributions = attemptDistributions,
            NumberOfStudents = numberOfStudents,
            SubmissionTimeDistribution = submissionTimeDistribution
        };

        return dto;
    }
}
