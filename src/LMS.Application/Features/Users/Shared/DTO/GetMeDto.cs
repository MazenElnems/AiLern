namespace LMS.Application.Features.Users.Shared.DTO;

public class GetMeDto
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Role { get; set; }
    public string? ImageUrl { get; set; }
}
