using Microsoft.AspNetCore.Http;

namespace LMS.Core.Services.Authentication.Interfaces;

public interface ICookieService
{
    void SetRefreshToken(HttpResponse response, string refreshToken, DateTime expiresOn);
    string? GetRefreshToken(HttpRequest request);
    void RemoveRefreshToken(HttpResponse response);
}
