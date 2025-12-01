namespace LMS.Core.DTOs.Auth.Response;

public class EmailConfirmationResponse(bool isConfirmed,string passwordToken ,int? userId = null)
{
    public bool IsConfirmed { get; set; } = isConfirmed;
    public int? UserId{ get; set; } = userId;
    public string PasswordToken { get; set; } = passwordToken;
}
