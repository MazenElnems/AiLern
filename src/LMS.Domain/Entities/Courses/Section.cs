using Microsoft.EntityFrameworkCore;

namespace LMS.Domain.Entities.Courses;

public class Section
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int SectionNumber { get; set; }

    // Foreign Keys
    public int CourseId { get; set; }

    // Navigation Properties
    public Course Course { get; set; } 
    public List<MaterialFile> MaterialFiles { get; set; } = new List<MaterialFile>();

    public void RemoveFile(MaterialFile file)
    {
        MaterialFiles.Remove(file);

        var filesToShift = MaterialFiles
            .Where(f => f.SectionId == file.SectionId &&
                   f.OrderIndex > file.OrderIndex).ToList();
        foreach (var f in filesToShift)
        {
            f.OrderIndex -= 1;
        }
    }

    public int GetMaxFileOrderIndexAsync()
    {
        return MaterialFiles
            .Max(f => (int?)f.OrderIndex) ?? 0;
    }
}