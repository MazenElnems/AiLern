using LMS.Domain.Entities.Assignments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Domain.Entities.EntitiesConfiguration;

public class AssignmentSubmissionEntityTypeConfiguration : IEntityTypeConfiguration<AssignmentSubmission>
{
    public void Configure(EntityTypeBuilder<AssignmentSubmission> builder)
    {
        builder.ToTable("AssignmentSubmissions");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.SubmissionDate)
            .HasColumnType("DATETIME2")
            .IsRequired();

        builder.Property(a => a.IsLate)
            .HasColumnType("BIT")
            .IsRequired();

        builder.Property(a => a.Feedback)
            .HasColumnType("NVARCHAR(MAX)")
            .IsRequired(false);

        builder.HasOne(a => a.Student)
            .WithMany(s => s.AssignmentSubmissions)
            .HasForeignKey(a => a.StudentId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Assignment)
            .WithMany(s => s.Submissions)
            .HasForeignKey(a => a.AssignmentId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
