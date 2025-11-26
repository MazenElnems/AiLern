using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.DTOs.Courses
{
    public class GetApprovedCoursesDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int InstructorId { get; set; }
        public int? SectionCourseId { get; set; }
        public string InstructorName { get; set; }
        public string? Course { get; set; }

    }
}
