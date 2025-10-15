using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Models.DTOs
{
    public class RegisterDto
    {
        [Required]
        [MaxLength(200)]
        public string UserName { get; set; }
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
}
