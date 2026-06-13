namespace LMS.Application.Features.Courses.Shared.DTO;

public class GetCourseDetailsDto
{
    public int Id { get; set; }
    public string? imagePath { get; set; }
    public string CourseName { get; set; }
    public string CourseCode { get; set; }
    public string? CourseDescription { get; set; }
    public int InstructorId { get; set; }
    public string InstructorName { get; set; }
    public string InstructorEmail { get; set; }
    public string? InstructorImage { get; set; }
    public int TotalEnrollments { get; set; }
    public int TotalMaterialNumber { get; set; }
    public long TotalMaterialSize { get; set; } // in bytes
    public int TotalAiResourcesNumber { get; set; }
    public long TotalAiResourcesSize { get; set; } // in bytes
}
