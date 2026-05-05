using LMS.Domain.Enums;

namespace LMS.Application.Contracts.Services;

public interface IAIServiceNotifier
{
    Task NotifyStatusChangeAsync(Guid fileId, string userId, AIStatus status, CancellationToken cancellationToken);
    Task NotifyQuestionGeneratedAsync(int userId, int questionsCount, bool completed, CancellationToken cancellationToken);
}
