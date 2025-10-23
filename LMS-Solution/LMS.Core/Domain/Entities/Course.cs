using LMS.Core.Domain.Enums;

namespace LMS.Core.Domain.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public CourseStatus CourseStatus { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime CreatedAt { get; set; }

        // Foreign Keys
        public int InstructorId { get; set; }
        public int? Approvedby { get; set; }
        public int? SectionCourseId { get; set; }

        // Navigation Properities
        public Instructor Instructor { get; set; }
        public Admin? Admin { get; set; }
        public Course? Section { get; set; }
    }
}
