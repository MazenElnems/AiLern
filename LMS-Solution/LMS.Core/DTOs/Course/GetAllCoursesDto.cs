using LMS.Core.Domain.Enums;

namespace LMS.Core.DTOs.Course;

public class GetAllCoursesDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public CourseStatus CourseStatus { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public int InstructorId { get; set; }
    public string InstructorName { get; set; }
    public int? Approvedby { get; set; }
    public int? SectionCourseId { get; set; }
    public string? SectionCourseName { get; set; }
}
