namespace LMS.Application.Contracts.Jobs;

public interface ICalculateStudentScoreJob
{
    Task ExecuteAsync(Guid attemptId, CancellationToken cancellationToken);
}
