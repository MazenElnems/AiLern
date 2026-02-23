namespace LMS.Domain.Interfaces;

public interface IRefreshTokenService
{
    string GenerateRefreshToken();
}
