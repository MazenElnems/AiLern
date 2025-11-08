namespace LMS.Core.CurrentUser;

public interface IUserContext
{
    CurrentUserIdentity? GetCurrentUser();
}
