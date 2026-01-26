using LMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Domain.Entities.EntitiesConfiguration;

public class AssignmentSubmissionFileEntityTypeConfiguration : IEntityTypeConfiguration<AssignmentSubmissionFile>
{
    public void Configure(EntityTypeBuilder<AssignmentSubmissionFile> builder)
    {
        builder.ToTable("AssignmentSubmissionFiles");

        builder.HasKey(af => af.Id);

        builder.Property(af => af.FileName)
            .HasColumnType("NVARCHAR(255)")
            .IsRequired();

        builder.Property(af => af.FileType)
            .HasColumnType("NVARCHAR(100)")
            .IsRequired();

        builder.Property(af => af.StoragePath)
            .HasColumnType("NVARCHAR(MAX)")
            .IsRequired();

        builder.Property(af => af.UploadStatus)
            .HasConversion<string>()
            .HasDefaultValue(UploadStatus.Pending);

        builder.HasOne(af => af.AssignmentSubmission)
            .WithMany(a => a.Files)
            .HasForeignKey(af => af.AssignmentSubmissionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
