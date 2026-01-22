using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Domain.Entities.EntitiesConfiguration;

public class AssignmentSubmissionFileEntityTypeConfiguration : IEntityTypeConfiguration<AssignmentSubmissionFile>
{
    public void Configure(EntityTypeBuilder<AssignmentSubmissionFile> builder)
    {
        builder.ToTable("AssignmentSubmissionFiles");

        builder.HasKey(af => af.FileId);

        builder.Property(af => af.FileName)
            .HasColumnType("NVARCHAR(255)")
            .IsRequired();

        builder.Property(af => af.StoragePath)
            .HasColumnType("NVARCHAR(MAX)")
            .IsRequired();

        builder.HasOne(af => af.AssignmentSubmission)
            .WithMany(a => a.Files)
            .HasForeignKey(af => af.AssignmentSubmissionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
