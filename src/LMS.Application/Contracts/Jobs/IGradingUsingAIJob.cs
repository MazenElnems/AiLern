namespace LMS.Application.Contracts.Jobs;

public interface IGradingUsingAIJob
{
    Task ExecuteAsync(Guid quizId, List<Guid> attemptIds, CancellationToken cancellationToken);
}
