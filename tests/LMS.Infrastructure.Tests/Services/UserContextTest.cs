using FluentAssertions;
using LMS.Application.CurrentUser;
using LMS.Domain.Constants;
using LMS.Domain.Exceptions;
using LMS.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace LMS.Infrastructure.Tests.Services;

public class UserContextTest
{
    [Theory]
    [InlineData(UserRoles.Student)]
    [InlineData(UserRoles.Admin)]
    [InlineData(UserRoles.Instructor)]
    public void GetCurrentUser_WhenUserIsAuthenticated_ShouldReturnValidClaimsPrincipal(string role)    
    {
        // Arrange
        var userId = "1";
        var userEmail = "m@gmail.com";
        var userName = "ma";

        var claims = new Claim[]
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, userEmail),
            new(ClaimTypes.Role, role),
            new(ClaimTypes.Name, userName)
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Bearer Token");

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipal
        };

        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };

        var expectedClaimsPrincipal = new CurrentUserIdentity(Convert.ToInt32(userId), userName, userEmail, [role]);

        var userContext = new UserContext(httpContextAccessor);

        // Act
        var actualClaimsPrincipal = userContext.GetCurrentUser();

        // Assert
        actualClaimsPrincipal.Should().BeEquivalentTo(expectedClaimsPrincipal);
    }

    [Fact]
    public void GetCurrentUser_WhenUserIdentityIsNull_ThrowsUnAuthorizedException()
    {
        var httpContext = new DefaultHttpContext
        {
            User = null
        };

        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };

        var userContext = new UserContext(httpContextAccessor);

        // Act
        var func = () => userContext.GetCurrentUser();
        
        // Assert
        func.Should().Throw<UnAuthorizedException>();
    }
}
