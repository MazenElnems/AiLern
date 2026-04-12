namespace LMS.Application.Contracts.Jobs;

public interface IAutoSubmitAttemptJob
{
    Task ExecuteAsync(Guid attemptId, CancellationToken cancellationToken);
}
