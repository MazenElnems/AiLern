namespace LMS.Application.Features.Users.Shared.DTO;

public class GetUserByIdDto
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string Role { get; set; }
    public string CreatedBy { get; set; }
}
