namespace LMS.Application.Contracts.Jobs;

public interface IPrepareDocumentsForAIJob
{
    Task ExecuteAsync(int courseId, int instructorId, CancellationToken cancellationToken);
}
