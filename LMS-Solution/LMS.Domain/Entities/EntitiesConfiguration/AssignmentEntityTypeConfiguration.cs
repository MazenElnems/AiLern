using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Domain.Entities.EntitiesConfiguration;

public class AssignmentEntityTypeConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> builder)
    {
        builder.ToTable("Assignments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title)
            .HasColumnType("NVARCHAR(200)")
            .IsRequired();

        builder.Property(a => a.Instructions)
            .HasColumnType("NVARCHAR(MAX)")
            .IsRequired();

        builder.Property(a => a.DueDate)
            .HasColumnType("DATETIME2")
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .HasColumnType("DATETIME2")
            .IsRequired();

        builder.Property(a => a.AllowLateSubmission)
            .HasColumnType("BIT")
            .IsRequired();

        builder.Property(a => a.IsPublished)
            .HasColumnType("BIT")
            .IsRequired();

        builder.HasOne(a => a.Course)
            .WithMany(c => c.Assignments)
            .HasForeignKey(a => a.CourseId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
