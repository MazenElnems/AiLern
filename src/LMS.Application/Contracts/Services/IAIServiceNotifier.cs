using LMS.Domain.Enums;

namespace LMS.Application.Contracts.Services;

public interface IAIServiceNotifier
{
    Task NotifyStatusChangeAsync(Guid fileId, string userId, AIStatus status, string? error, CancellationToken cancellationToken);
    Task NotifyQuestionGeneratedAsync(int userId, int questionsCount, bool completed, CancellationToken cancellationToken);
    Task NotifyQuestionGenerationFailedAsync(int userId, string? error, CancellationToken cancellationToken);
    Task NotifyAIServiceProblemAsync(string userId, string? error, CancellationToken cancellationToken);
}
