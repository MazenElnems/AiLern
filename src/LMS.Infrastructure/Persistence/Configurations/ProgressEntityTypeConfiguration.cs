using LMS.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public class ProgressConfiguration : IEntityTypeConfiguration<CourseProgress>
{
    public void Configure(EntityTypeBuilder<CourseProgress> builder)
    {
        builder.ToTable("StudentCourseProgress");

        builder.HasKey(p => new { p.CourseId, p.StudentId });

        builder.Property(p => p.IsCompleted)
               .IsRequired();

        builder.Property(p => p.Type)
               .IsRequired()
               .HasConversion<string>(); 

        builder.Property(p => p.LastOpenedFileId)
               .IsRequired(false);

        builder.Property(p => p.LastPageNumber)
               .IsRequired(false);

        builder.Property(p => p.LastWatchedTime)
               .IsRequired(false);

        builder.HasOne(p => p.Course)
               .WithMany(c => c.Progresses)
               .HasForeignKey(p => p.CourseId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Student)
               .WithMany(s => s.Progresses)
               .HasForeignKey(p => p.StudentId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
