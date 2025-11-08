using System.ComponentModel.DataAnnotations;

namespace LMS.Core.DTOs.Users;

public class GetUserByIdDto
{
    [Required]
    [MaxLength(200)]
    public string UserName { get; set; }
    [Required]
    [MaxLength(200)]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string FullName { get; set; }
    [Required]
    public string Role { get; set; }
    [Required]
    public string CreatedBy { get; set; }



}
