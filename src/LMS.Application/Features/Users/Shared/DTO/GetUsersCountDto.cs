namespace LMS.Application.Features.Users.Shared.DTO;

public class GetUsersCountDto
{
    public int TotalUsers { get; set; }
    public int TotalStudent { get; set; }
    public int TotalInstructors { get; set; }
    public int TotalAdmins { get; set; }
}
