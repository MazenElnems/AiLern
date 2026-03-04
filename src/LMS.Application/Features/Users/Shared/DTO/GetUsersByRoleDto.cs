namespace LMS.Application.Features.Users.Shared.DTO;

public class GetUsersByRoleDto
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string CreatedBy { get; set; }
    public string Role { get; set; }
}