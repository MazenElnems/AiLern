using LMS.Core.DTOs.Authentication;
using LMS.Core.Models;
using LMS.Core.Services.Authentication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICookieService _cookieService;

        public AuthController(IAuthService authService, ICookieService cookieService)
        {
            _authService = authService;
            _cookieService = cookieService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await _authService.GetTokenAsync(loginDto);

            if (!result.IsSuccess)
                return BadRequest(new { result.Message, isAuthenticated = false, result.Errors });

            _cookieService.SetRefreshToken(Response, result.Data.RefreshToken, result.Data.RefreshTokenExpiration);

            return Ok(result.Data);
        }

        [HttpPost("register")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var result = await _authService.CreateUserAsync(username, registerDto);

            if (!result.IsSuccess)
                return BadRequest(new { result.Message, result.Errors });

            return Ok(new { isAuthenticated = true, result.Message });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = _cookieService.GetRefreshToken(Request);

            var result = await _authService.GetRefreshTokenAsync(refreshToken);

            if (!result.IsSuccess)
                return BadRequest(new
                {
                    result.Message,
                    result.Errors
                });

            _cookieService.SetRefreshToken(Response, result.Data.RefreshToken, result.Data.RefreshTokenExpiration);

            return Ok(result.Data);
        }

        [HttpPut("revoke-token")]
        public async Task<IActionResult> RevokeRefreshToken()
        {
            var refreshToken = _cookieService.GetRefreshToken(Request);

            var result = await _authService.RevokeRefreshTokenAsync(refreshToken);

            if (!result.IsSuccess)
                return BadRequest(new
                {
                    result.Message,
                    result.Errors
                });

            _cookieService.RemoveRefreshToken(Response);

            return Ok(new { result.Message });
        }

    }
}
