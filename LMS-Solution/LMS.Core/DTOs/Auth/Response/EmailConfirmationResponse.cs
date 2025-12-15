namespace LMS.Core.DTOs.Auth.Response;

public class EmailConfirmationResponse(bool isConfirmed,string passwordToken ,string? email = null)
{
    public bool IsConfirmed { get; set; } = isConfirmed;
    public string Email{ get; set; } = email;
    public string PasswordToken { get; set; } = passwordToken;
}
