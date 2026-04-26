using LMS.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Persistence.Configurations
{
    public class ProgressConfiguration : IEntityTypeConfiguration<Progress>
    {
        public void Configure(EntityTypeBuilder<Progress> builder)
        {
            builder.ToTable("StudentCourseProgress");

            builder.HasKey(p => new { p.CourseId, p.StudentId });

            builder.Property(p => p.IsCompleted)
                   .IsRequired();

            builder.Property(p => p.UpdatedAt)
                   .IsRequired();

            builder.Property(p => p.Percent)
                   .IsRequired();

            builder.Property(p => p.Type)
                   .IsRequired()
                   .HasConversion<string>(); 

            builder.Property(p => p.LastLearningItemId)
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
}
