namespace LMS.Domin.DTOs.Students;

public class GetStudentsByCourseIdDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
}
