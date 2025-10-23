using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.DTOs.Authentication
{
    public class LoginDto
    {
        [Required]
        [MaxLength(200)]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
