using LMS.Domain.Common.Enums;
using LMS.Domain.Entities.Assignments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Domain.Entities.EntitiesConfiguration;

public class AssignmentFileEntityTypeConfiguration : IEntityTypeConfiguration<AssignmentFile>
{
    public void Configure(EntityTypeBuilder<AssignmentFile> builder)
    {
        builder.ToTable("AssignmentFiles");

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

        builder.HasOne(af => af.Assignment)
            .WithMany(a => a.Files)
            .HasForeignKey(af => af.AssignmentId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
