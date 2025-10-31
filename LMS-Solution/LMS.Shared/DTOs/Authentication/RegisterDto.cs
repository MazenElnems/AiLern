using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs.Authentication;

public class RegisterDto
{
    [Required]
    [MaxLength(200)]
    public string UserName { get; set; }
    [Required]
    [MaxLength(200)]
    [EmailAddress]
    public string Email { get; set;  }
    [Required]
    public string FullName { get; set; }
    [Required]
    [MinLength(6)]
    public string Password { get; set; }
    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
    [Required]
    public string Role { get; set; }
}
