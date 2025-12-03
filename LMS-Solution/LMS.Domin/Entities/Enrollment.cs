using LMS.Domin.Enums;

namespace LMS.Domin.Entities;

public class Enrollment
{
    public int Course_id { get; set; }
    public int Student_id { get; set; }
    public EnrollmentStatus Status { get; set; }
    public DateTime Requested_at { get; set; }


    // Navigation Properities
    public Student Student { get; set; }
    public Course Course { get; set; }

}
