namespace LMS.Application.Contracts.ExternalServices;

public interface IDbInitializer
{
    Task InitializeAsync();
}
