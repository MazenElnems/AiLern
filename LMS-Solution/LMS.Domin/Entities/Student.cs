namespace LMS.Domin.Entities;

public class Student : ApplicationUser
{
    public int StudentId { get; set; }

    // Navigation Properities
    public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
