namespace LMS.Application.Contracts.Jobs;

public interface IQuizPublishSchedulerJob
{
    Task ExecuteAsync(Guid quizId);
}
