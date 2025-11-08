using LMS.Domin.Entities;
using LMS.Domin.Enums;
using LMS.Infrastructure.Data;
using LMS.Infrastructure.Seeders.Interfaces;

namespace LMS.Infrastructure.Seeders;

public class DataSeeder : IDataSeeder
{
    private readonly AppDbContext _db;

    public DataSeeder(AppDbContext db)
    {
        _db = db;
    }

    public async Task SeedAsync()
    {
        if(_db.Database.CanConnect())
        {
            if(!_db.Courses.Any())
            {
                var courses = LoadCourses();
                _db.Courses.AddRange(courses);
                await _db.SaveChangesAsync();
            }
        }
    }

    private List<Course> LoadCourses()
    {
        var courses = new List<Course>
        {
            new Course
            {
                Code = "CS101",
                Name = "Introduction to Programming",
                Description = "Learn the basics of programming using C# and .NET.",
                CourseStatus = CourseStatus.Approved,
                ApprovedDate = DateTime.UtcNow.AddDays(-20),
                CreatedAt = DateTime.UtcNow.AddDays(-40),
                InstructorId = 2,
                Approvedby = 1
            },
            new Course
            {
                Code = "CS102",
                Name = "Object-Oriented Programming",
                Description = "Master OOP principles with practical examples in C#.",
                CourseStatus = CourseStatus.Approved,
                ApprovedDate = DateTime.UtcNow.AddDays(-18),
                CreatedAt = DateTime.UtcNow.AddDays(-38),
                InstructorId = 2,
                Approvedby = 1
            },
            new Course
            {
                Code = "CS103",
                Name = "Data Structures",
                Description = "Learn how data is organized, accessed, and manipulated efficiently.",
                CourseStatus = CourseStatus.Approved,
                ApprovedDate = DateTime.UtcNow.AddDays(-15),
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                InstructorId = 2,
                Approvedby = 1
            },
            new Course
            {
                Code = "CS104",
                Name = "Database Systems",
                Description = "Explore relational databases, SQL, and normalization.",
                CourseStatus = CourseStatus.Approved,
                ApprovedDate = DateTime.UtcNow.AddDays(-12),
                CreatedAt = DateTime.UtcNow.AddDays(-28),
                InstructorId = 2,
                Approvedby = 1
            },
            new Course
            {
                Code = "CS105",
                Name = "Web Development Fundamentals",
                Description = "Understand the building blocks of modern web apps using HTML, CSS, and JavaScript.",
                CourseStatus = CourseStatus.Approved,
                ApprovedDate = DateTime.UtcNow.AddDays(-10),
                CreatedAt = DateTime.UtcNow.AddDays(-25),
                InstructorId = 2,
                Approvedby = 1
            },
            new Course
            {
                Code = "CS106",
                Name = "ASP.NET Core MVC",
                Description = "Build scalable web applications using ASP.NET Core MVC framework.",
                CourseStatus = CourseStatus.Approved,
                ApprovedDate = DateTime.UtcNow.AddDays(-8),
                CreatedAt = DateTime.UtcNow.AddDays(-22),
                InstructorId = 2,
                Approvedby = 1
            },
            new Course
            {
                Code = "CS107",
                Name = "Entity Framework Core",
                Description = "Learn ORM concepts and data access using EF Core.",
                CourseStatus = CourseStatus.Approved,
                ApprovedDate = DateTime.UtcNow.AddDays(-6),
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                InstructorId = 2,
                Approvedby = 1
            },
            new Course
            {
                Code = "CS108",
                Name = "Software Engineering Principles",
                Description = "Understand software design, testing, and development lifecycle models.",
                CourseStatus = CourseStatus.Pending,
                ApprovedDate = null,
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                InstructorId = 2
            },
            new Course
            {
                Code = "CS109",
                Name = "Design Patterns in C#",
                Description = "Learn common software design patterns like Singleton, Factory, and Observer.",
                CourseStatus = CourseStatus.Pending,
                ApprovedDate = null,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                InstructorId = 2
            },
            new Course
            {
                Code = "CS110",
                Name = "RESTful API Development",
                Description = "Develop clean and scalable REST APIs using ASP.NET Core Web API.",
                CourseStatus = CourseStatus.Approved,
                ApprovedDate = DateTime.UtcNow.AddDays(-5),
                CreatedAt = DateTime.UtcNow.AddDays(-18),
                InstructorId = 2,
                Approvedby = 1
            },
            new Course
            {
                Code = "CS111",
                Name = "Cloud Computing Basics",
                Description = "Introduction to cloud concepts, services, and deployment models.",
                CourseStatus = CourseStatus.Approved,
                ApprovedDate = DateTime.UtcNow.AddDays(-3),
                CreatedAt = DateTime.UtcNow.AddDays(-16),
                InstructorId = 2,
                Approvedby = 1
            },
            new Course
            {
                Code = "CS112",
                Name = "Machine Learning Fundamentals",
                Description = "Learn the core principles and algorithms behind modern AI systems.",
                CourseStatus = CourseStatus.Pending,
                ApprovedDate = null,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                InstructorId = 2
            },
            new Course
            {
                Code = "CS113",
                Name = "Data Analytics with Python",
                Description = "Analyze data effectively using Python libraries like Pandas and NumPy.",
                CourseStatus = CourseStatus.Rejected,
                ApprovedDate = null,
                CreatedAt = DateTime.UtcNow.AddDays(-8),
                InstructorId = 2
            },
            new Course
            {
                Code = "CS114",
                Name = "Cybersecurity Essentials",
                Description = "Learn how to protect systems and networks from digital threats.",
                CourseStatus = CourseStatus.Approved,
                ApprovedDate = DateTime.UtcNow.AddDays(-2),
                CreatedAt = DateTime.UtcNow.AddDays(-12),
                InstructorId = 2,
                Approvedby = 1
            },
            new Course
            {
                Code = "CS115",
                Name = "DevOps & CI/CD Pipelines",
                Description = "Master automation, integration, and deployment pipelines with DevOps tools.",
                CourseStatus = CourseStatus.Approved,
                ApprovedDate = DateTime.UtcNow.AddDays(-1),
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                InstructorId = 2,
                Approvedby = 1
            }
        };

        return courses;
    }
}
