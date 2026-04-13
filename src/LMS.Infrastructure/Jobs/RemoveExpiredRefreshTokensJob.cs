using LMS.Application.Contracts.Jobs;
using LMS.Application.Contracts.UnitOfWork;

namespace LMS.Infrastructure.Jobs;

public class RemoveExpiredRefreshTokensJob : IRemoveExpiredRefreshTokensJob
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveExpiredRefreshTokensJob(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync()
    {
        await _unitOfWork.RefreshTokens.RemoveExpiredRefreshTokensAsync();
        await _unitOfWork.CommitAsync();
    }
}
