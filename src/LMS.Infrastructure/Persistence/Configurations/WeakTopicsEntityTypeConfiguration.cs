using LMS.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public class WeakTopicsEntityTypeConfiguration : IEntityTypeConfiguration<WeakTopic>
{
    public void Configure(EntityTypeBuilder<WeakTopic> builder)
    {
        builder
            .ToTable("WeakTopics");

        builder
            .HasOne(t => t.Attempt)
            .WithMany(a => a.WeakTopics)
            .HasForeignKey(t => t.AttemptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(t => t.Course)
            .WithMany(a => a.WeakTopics)
            .HasForeignKey(t => t.CourseId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
