namespace LMS.Domain.Interfaces;

public interface IQuizPublishSchedulerJob
{
    Task ExecuteAsync(Guid quizId);
}
