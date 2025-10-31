namespace LMS.Core.Users;

public record CurrentUser(int Id,string UserName, string Email,IEnumerable<string> Roles)
{
    public bool IsInRole(string RoleName) => Roles.Contains(RoleName);
}
