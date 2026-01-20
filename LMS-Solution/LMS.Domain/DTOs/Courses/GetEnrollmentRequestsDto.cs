namespace LMS.Domain.DTOs.Courses;

public class GetEnrollmentRequestsDto
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public int StudentId { get; set; }
    public DateTime RequestAt { get; set; }
}
