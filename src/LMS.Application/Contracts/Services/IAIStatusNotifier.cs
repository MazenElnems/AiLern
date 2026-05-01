using LMS.Domain.Enums;

namespace LMS.Application.Contracts.Services;

public interface IAIStatusNotifier
{
    Task NotifyStatusChangeAsync(Guid fileId, string userId, AIStatus status, CancellationToken cancellationToken);
}
