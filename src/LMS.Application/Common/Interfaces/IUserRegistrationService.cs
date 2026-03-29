using LMS.Application.Common.Results.Generic;
using LMS.Domain.Entities.Users;

namespace LMS.Application.Common.Interfaces;

public interface IUserRegistrationService
{
    Task<Result<int>> RegisterUserAsync(ApplicationUser user, string password, string role);
}
