namespace LMS.Application.Features.Students.Shared.DTO;

public class GetEnrolledStudentsDto
{
    public int StudentId { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime EnrolledAt { get; set; }
}
