using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs.Authentication;

public class LoginDto
{
    [Required]
    [MaxLength(200)]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
}
