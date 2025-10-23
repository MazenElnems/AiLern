using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Services.Authentication.Interfaces
{
    public interface ICookieService
    {
        void SetRefreshToken(HttpResponse response, string refreshToken, DateTime expiresOn);
        string? GetRefreshToken(HttpRequest request);
        void RemoveRefreshToken(HttpResponse response);
    }
}
