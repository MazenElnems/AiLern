using LMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Domain.Entities.EntitiesConfiguration;

public class CourseEntityTypeConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder
            .ToTable("Courses");

        builder
            .HasKey(c => c.Id);

        builder
            .HasIndex(c => c.Code)
            .IsUnique();

        builder
            .HasIndex(c => c.Name)
            .IsUnique();

        builder.Property(c => c.Code)
            .HasColumnType("VARCHAR(10)")
            .IsRequired();

        builder.Property(c => c.Name)
            .HasColumnType("VARCHAR(50)")
            .IsRequired();

        builder
            .Property(c => c.Description)
            .HasColumnType("NVARCHAR(MAX)")
            .IsRequired(false);

        builder.HasOne(c => c.Instructor)
            .WithMany(i => i.Courses)
            .HasForeignKey(c => c.InstructorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(c => c.CreatedAt)
            .HasColumnType("DATETIME2")
            .IsRequired();

        builder.Property(c => c.CourseStatus)
            .HasConversion<string>()
            .HasDefaultValue(CourseStatus.Pending);
    }
}
