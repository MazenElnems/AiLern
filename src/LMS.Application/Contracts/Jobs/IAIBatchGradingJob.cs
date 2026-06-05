namespace LMS.Application.Contracts.Jobs;

public interface IAIBatchGradingJob
{
    Task ExecuteAsync(int CourseId, Guid quizId, List<Guid> attemptIds, CancellationToken cancellationToken);
}
