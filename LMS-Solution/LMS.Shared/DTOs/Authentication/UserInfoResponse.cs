namespace LMS.Shared.DTOs.Authentication;

public class UserInfoResponse
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public DateTime CreatedAt { get; set; }
}
