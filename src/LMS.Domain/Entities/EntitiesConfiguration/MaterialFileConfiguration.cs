using LMS.Domain.Common.Enums;
using LMS.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS.Domain.Entities.EntitiesConfiguration;

public class MaterialFileConfiguration : IEntityTypeConfiguration<MaterialFile>
{
    public void Configure(EntityTypeBuilder<MaterialFile> builder)
    {
        builder.ToTable("MaterialFiles");

        builder.HasKey(mf => mf.Id);

        builder.Property(mf => mf.FileName)
            .HasColumnType("NVARCHAR(255)")
            .IsRequired();

        builder.Property(mf => mf.UploadStatus)
            .HasConversion<string>()
            .HasColumnType("NVARCHAR(50)")
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

        builder.HasIndex(f => f.SectionId); // FK

        builder.HasIndex(f => new { f.SectionId, f.OrderIndex }).IsUnique(); // reorder

        builder.HasOne(mf => mf.Section)
            .WithMany(s => s.MaterialFiles)
            .HasForeignKey(mf => mf.SectionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}