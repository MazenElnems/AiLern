namespace LMS.Domain.Interfaces;

public interface IAutoSubmitAttemptJob
{
    Task ExecuteAsync(Guid attemptId);
}
