using LMS.Domain.Enums;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;

namespace LMS.Infrastructure.Jobs;

internal class QuizPublishSchedulerJob : IQuizPublishSchedulerJob
{
    private readonly IUnitOfWork _unitOfWork;

    public QuizPublishSchedulerJob(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid quizId)
    {
        var quiz = await _unitOfWork.Quizzes.GetByIdAsync(quizId);

        if (quiz == null)
            return;

        if(quiz.Status == QuizStatus.Scheduled)
        {
            quiz.Status = QuizStatus.Published;
            await _unitOfWork.CommitAsync();
        }
    }
}
