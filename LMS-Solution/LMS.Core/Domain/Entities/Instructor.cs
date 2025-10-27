using LMS.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Shared.Domain.Entities
{
    public class Instructor : ApplicationUser
    {
        // Navigation Properities
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}
