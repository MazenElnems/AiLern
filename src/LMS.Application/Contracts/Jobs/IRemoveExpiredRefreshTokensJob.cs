namespace LMS.Application.Contracts.Jobs;

public interface IRemoveExpiredRefreshTokensJob
{
    Task ExecuteAsync();
}
