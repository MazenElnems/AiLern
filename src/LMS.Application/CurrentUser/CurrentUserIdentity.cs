namespace LMS.Application.CurrentUser;

public record CurrentUserIdentity(int Id,string UserName, string Email,IEnumerable<string> Roles)
{
    public bool IsInRole(string RoleName) => Roles.Contains(RoleName);
}
