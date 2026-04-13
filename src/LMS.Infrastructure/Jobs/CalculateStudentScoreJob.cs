using LMS.Application.Contracts.Jobs;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Jobs;

internal class CalculateStudentScoreJob : ICalculateStudentScoreJob
{
    private readonly IUnitOfWork _unitOfWork;

    public CalculateStudentScoreJob(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid attemptId, CancellationToken cancellationToken)
    {
        var answers = await _unitOfWork.Answers.Query
            .Include(a => a.Question)
                .ThenInclude(q => q.Options)
            .Where(a => a.AttemptId == attemptId)
            .ToListAsync(cancellationToken);

        if (answers is null || answers.Count() == 0)
            return;

        foreach(var answer in answers)
        {
            var question = answer.Question;

            if (question.Type == QuestionType.Written)
                return;

            var correctOption = question.Options.First(o => o.IsCorrect);

            answer.Mark = answer.OptionId == correctOption.OptionId ? question.Mark : 0;
        }

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
