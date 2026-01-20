namespace LMS.Domain.DTOs.Users;

public class GetUserByIdDto
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string Role { get; set; }
    public string CreatedBy { get; set; }
}
