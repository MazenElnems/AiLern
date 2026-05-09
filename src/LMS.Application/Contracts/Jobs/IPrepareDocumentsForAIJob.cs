namespace LMS.Application.Contracts.Jobs;

public interface IPrepareDocumentsForAIJob
{
    Task ExecuteAsync(int courseId, CancellationToken cancellationToken);
}
