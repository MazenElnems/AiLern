using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Quizzes.Shared.DTO;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Attempts.Queries.GetAttemptsByQuizId;

public class GetAttemptsByQuizIdQueryHandler : IRequestHandler<GetAttemptsByQuizIdQuery, Result<GetAttemptsByQuizIdDto>>
{
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;

    public GetAttemptsByQuizIdQueryHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    {
        _userContext = userContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GetAttemptsByQuizIdDto>> Handle(GetAttemptsByQuizIdQuery request, CancellationToken cancellationToken)
    {
        var studentId = _userContext.GetCurrentUser().Id;

        var quiz = await _unitOfWork.Quizzes.Query
            .Select(q => new 
            {
                q.Id,
                q.Title,
                q.Status,
                q.CourseId,
                TotalPoints = q.Questions.Sum(q => q.Mark),
                q.AvailableFrom,
                q.AvailableUntil,
                q.ShowResultOnClose
            })
            .FirstOrDefaultAsync(q => q.Id == request.QuizId, cancellationToken);

        if (quiz == null)
            return DomainErrors.Quiz.NotFound(request.QuizId);

        if (!await _unitOfWork.Enrollments.IsEnrolledAsync(quiz.CourseId, studentId))
            return DomainErrors.Course.NotEnrolled;

        var attempts = await _unitOfWork.Attempts.Query
            .Include(a => a.Answers)
            .Where(a => a.StudentId == studentId && a.QuizId == request.QuizId)
            .OrderByDescending(a => a.SubmittedAt)
            .ToListAsync(cancellationToken);

        var myAttempts = new GetAttemptsByQuizIdDto
        {
            QuizId = quiz.Id,
            QuizTitle = quiz.Title,
            TotalPoints = quiz.TotalPoints,
            AvailableFrom = quiz.AvailableFrom,
            AvailableUntil = quiz.AvailableUntil,
            ShowResultOnClose = quiz.ShowResultOnClose,
            Attempts = attempts.Select(a => new AttemptMetaData
            {
                Id = a.Id,
                AttemptNumber = a.AttemptNumber,
                StartAt = a.StartAt,
                SubmittedAt = a.SubmittedAt,
                AttemptEndTime = a.AttemptEndTime,
                TimeSpent = a.TimeSpent,
                Status = a.Status,
                Score = quiz.AvailableUntil < DateTime.UtcNow && (a.Status == AttemptStatus.Reviewed || quiz.ShowResultOnClose) ? a.Score : null
            }).ToList()
        };

        return myAttempts;
    }
}
