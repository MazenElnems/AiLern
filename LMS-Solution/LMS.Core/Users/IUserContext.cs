namespace LMS.Core.Users;

public interface IUserContext
{
    CurrentUser? GetCurrentUser();
}
