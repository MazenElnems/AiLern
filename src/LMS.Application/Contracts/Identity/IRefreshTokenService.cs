namespace LMS.Application.Contracts.Identity;

public interface IRefreshTokenService
{
    string GenerateRefreshToken();
}
