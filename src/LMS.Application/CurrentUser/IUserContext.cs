namespace LMS.Application.CurrentUser;

public interface IUserContext
{
    CurrentUserIdentity GetCurrentUser();
}
