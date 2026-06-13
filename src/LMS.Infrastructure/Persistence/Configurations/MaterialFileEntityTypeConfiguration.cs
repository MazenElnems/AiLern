using LMS.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public class MaterialFileEntityTypeConfiguration : IEntityTypeConfiguration<MaterialFile>
{
    public void Configure(EntityTypeBuilder<MaterialFile> builder)
    {
        builder.ToTable("MaterialFiles");

        builder.HasKey(mf => mf.Id);

        builder.Property(mf => mf.UploadStatus)
            .HasConversion<string>()
            .HasColumnType("NVARCHAR(50)")
            .IsRequired();

        builder.Property(mf => mf.FileName)
            .HasColumnType("NVARCHAR(255)")
            .IsRequired();

        builder.Property(mf => mf.UploadDate)
            .HasColumnType("DATETIME2")
            .IsRequired();

        builder.Property(mf => mf.OrderIndex)
            .HasColumnType("INT")
            .IsRequired();

        builder.Property(mf => mf.StoragePath)
            .HasColumnType("NVARCHAR(500)")
            .IsRequired();

        builder.Property(mf => mf.FileType)
            .HasColumnType("NVARCHAR(50)")
            .IsRequired();

        builder.Property(mf => mf.FileSize)
            .HasColumnType("BIGINT")
            .IsRequired();

        builder.HasIndex(mf => mf.SectionId);

        builder.HasIndex(mf => new { mf.SectionId, mf.OrderIndex }).IsUnique();
    }
}
