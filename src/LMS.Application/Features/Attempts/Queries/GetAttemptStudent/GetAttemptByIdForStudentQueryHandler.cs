using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Attempts.Shared.DTO;
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
                .Where(a => a.Id == request.Id && a.StudentId == userId && a.Status != AttemptStatus.InProgress) /* a.Status != AttemptStatus.InProgress هنا انا عامل كدا علشان اجيب برضوا حاله ال Submitted الي مش معمل انك تشوف الاجابه علطول بعد الاؤسال ف هيجيب الاجابات بتاعت الطالب */
                .Select(a => new GetAttemptByIdDto /* ابقي رن عليا لو مفهمتش قبل م تمسح */
                {
                    QuizId = a.QuizId,
                    StudentId = a.StudentId,
                    Status = a.Status,
                    QuizName = a.Quiz.Title,
                    TotalScore = a.Quiz.Questions.Sum(q => q.Mark),
                    AchievedScore = (a.Status == AttemptStatus.Reviewed || 
                                    (a.Status == AttemptStatus.Submitted && a.Quiz.ShowResultOnClose == true)) 
                                    ? a.AttemptAnswers.Sum(aa => aa.Mark) : 0,
                    AttemptResult = a.AttemptAnswers.Select(aa => new AttemptResultDto
                    {
                        QuestionId = aa.QuestionId,
                        QuestionText = aa.Question.QuestionText,
                        MaxScore = aa.Question.Mark,
                        StudentAnswer = aa.WrittenAnswer
                                     ?? aa.BooleanAnswer
                                     ?? aa.OptionNumber.ToString()!,
                        Feedback = (a.Status == AttemptStatus.Reviewed || 
                                   (a.Status == AttemptStatus.Submitted && a.Quiz.ShowResultOnClose == true)) 
                                   ? aa.Feedback! : null!,
                        Score =  (a.Status == AttemptStatus.Reviewed ||
                                 (a.Status == AttemptStatus.Submitted && a.Quiz.ShowResultOnClose == true))
                                 ? aa.Mark : 0,
                        CorrectAnswer = (a.Status == AttemptStatus.Reviewed ||
                                        (a.Status == AttemptStatus.Submitted && a.Quiz.ShowResultOnClose == true)) 
                                        ? aa.Question.Options
                                            .Where(qo => qo.IsCorrect)
                                            .Select(qo => qo.OptionText)
                                            .FirstOrDefault()! : null!,
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
