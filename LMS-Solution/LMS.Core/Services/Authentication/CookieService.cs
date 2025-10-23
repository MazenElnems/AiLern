using LMS.Core.Services.Authentication.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LMS.Core.Services.Authentication
{
    public class CookieService : ICookieService
    {
        private string _cookieKey = "refresh-token";
        public string? GetRefreshToken(HttpRequest request)
        {
            return request.Cookies[_cookieKey];
        }

        public void RemoveRefreshToken(HttpResponse response)
        {
            response.Cookies.Delete(_cookieKey);
        }

        public void SetRefreshToken(HttpResponse response, string refreshToken, DateTime expiresOn)
        {
            response.Cookies.Append(_cookieKey, refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Expires = expiresOn,
                Path = "api/auth/"
            });
        }
    }
}
