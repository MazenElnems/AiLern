using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Attempts.Shared.DTO;
using LMS.Application.Features.Quizzes.Shared.Requests;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Attempts.Queries.GetAttempt;

public class GetAttemptByIdForStudentQueryHandler : IRequestHandler<GetAttemptByIdForStudentQuery, Result<GetAttemptByIdDto>>
{
    private readonly IUserContext _user;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetAttemptByIdForStudentQueryHandler> _logger;

    public GetAttemptByIdForStudentQueryHandler(IUserContext user, IUnitOfWork unitOfWork, ILogger<GetAttemptByIdForStudentQueryHandler> logger)
    {
        _user = user;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<GetAttemptByIdDto>> Handle(GetAttemptByIdForStudentQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _user.GetCurrentUser().Id;
            var attempt = await _unitOfWork.Attempts.Query
                .AsNoTracking()
                .Where(a => a.Id == request.Id && a.StudentId == userId && a.Status != AttemptStatus.InProgress)
                .Select(a => new GetAttemptByIdDto 
                {
                    QuizId = a.QuizId,
                    StudentId = a.StudentId,
                    Status = a.Status,
                    QuizName = a.Quiz.Title,
                    TotalScore = a.Quiz.Questions.Sum(q => q.Mark),
                    AchievedScore = (a.Status == AttemptStatus.Reviewed || 
                                    (a.Status == AttemptStatus.Submitted && a.Quiz.ShowResultOnClose && DateTime.UtcNow >= a.Quiz.AvailableUntil)) 
                                    ? a.AttemptAnswers.Sum(aa => aa.Mark) : 0,
                    AttemptResult = a.AttemptAnswers.Select(aa => new AttemptResultDto
                    {
                        QuestionId = aa.QuestionId,
                        QuestionText = aa.Question.QuestionText,
                        Type = aa.Question.Type.ToString(),
                        MaxScore = aa.Question.Mark,
                        StudentAnswer = aa.WrittenAnswer
                                     ?? aa.BooleanAnswer
                                     ?? aa.OptionNumber.ToString()!,
                        Feedback = (a.Status == AttemptStatus.Reviewed ||
                                   (a.Status == AttemptStatus.Submitted && a.Quiz.ShowResultOnClose && DateTime.UtcNow >= a.Quiz.AvailableUntil))
                                   ? aa.Feedback! : null!,
                        Options = aa.Question.Options
                            .Select(o => new OptionDto 
                            { 
                                IsCorrect = (a.Status == AttemptStatus.Reviewed || (a.Status == AttemptStatus.Submitted && a.Quiz.ShowResultOnClose && DateTime.UtcNow >= a.Quiz.AvailableUntil)) 
                                    ? o.IsCorrect
                                    :false,
                                OptionText = o.OptionText 
                            }).ToList(),
                        Instructions = aa.Question.Instructions,
                        Explanation = aa.Question.Explanation,
                        Score =  (a.Status == AttemptStatus.Reviewed ||
                                 (a.Status == AttemptStatus.Submitted && a.Quiz.ShowResultOnClose && DateTime.UtcNow >= a.Quiz.AvailableUntil))
                                 ? aa.Mark : 0
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
