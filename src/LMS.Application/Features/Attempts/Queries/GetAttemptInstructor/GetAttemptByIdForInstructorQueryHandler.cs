using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Attempts.Shared.DTO;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Attempts.Queries.GetAttemptInstructor;

public class GetAttemptByIdForInstructorQueryHandler : IRequestHandler<GetAttemptByIdForInstructorQuery, Result<GetAttemptByIdDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _user;
    private readonly ILogger<GetAttemptByIdForInstructorQueryHandler> _logger;

    public GetAttemptByIdForInstructorQueryHandler(IUnitOfWork unitOfWork, IUserContext user, ILogger<GetAttemptByIdForInstructorQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _user = user;
        _logger = logger;
    }

    public async Task<Result<GetAttemptByIdDto>> Handle(GetAttemptByIdForInstructorQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _user.GetCurrentUser().Id;
            var attempt = await _unitOfWork.Attempts.Query
                .AsNoTracking()
                .Where(a => a.Id == request.Id && a.Quiz.Course.InstructorId == userId && a.Status != AttemptStatus.InProgress )
                .Select(a => new GetAttemptByIdDto
                {
                    QuizId = a.QuizId,
                    QuizName = a.Quiz.Title,
                    StudentId = a.StudentId,
                    Status = a.Status,
                    TotalScore = a.Quiz.Questions.Sum(q => q.Mark),
                    AchievedScore = a.AttemptAnswers.Sum(aa => aa.Mark),
                    AttemptResult = a.AttemptAnswers.Select(aa => new AttemptResultDto
                    {
                        QuestionId = aa.QuestionId,
                        QuestionText = aa.Question.QuestionText,
                        MaxScore = aa.Question.Mark,
                        StudentAnswer = aa.WrittenAnswer
                                         ?? aa.BooleanAnswer
                                         ?? aa.OptionNumber.ToString()!,
                        Feedback = aa.Feedback!,
                        Score = aa.Mark,
                        CorrectAnswer = aa.Question.Options
                                        .Where(qo => qo.IsCorrect)
                                        .Select(qo => qo.OptionText)
                                        .FirstOrDefault()!,
                    }).ToList()

                }).FirstOrDefaultAsync(cancellationToken);
            if (attempt == null)
                return DomainErrors.Attempt.NotFound(request.Id);
            return attempt;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving attempt.");
            throw;
        }

    }
}
