using LMS.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Domain.Entities.EntitiesConfiguration;

public class SectionConfiguration : IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        builder.ToTable("Sections");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Title)
            .HasColumnType("NVARCHAR(200)")
            .IsRequired();


        builder.HasOne(s => s.Course)
            .WithMany(c =>c.Sections)
            .HasForeignKey(s => s.CourseId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsMany(s => s.MaterialFiles, c =>
        {
            c.HasKey(x => x.Id);

            c.ToTable("MaterialFiles");

            c.Property(mf => mf.UploadStatus)
                .HasConversion<string>()
                .HasColumnType("NVARCHAR(50)")
                .IsRequired();

            c.Property(mf => mf.FileName)
                .HasColumnType("NVARCHAR(255)")
                .IsRequired();

            c.Property(mf => mf.UploadDate)
                .HasColumnType("DATETIME2")
                .IsRequired();

            c.Property(mf => mf.OrderIndex)
                .HasColumnType("INT")
                .IsRequired();

            c.Property(mf => mf.StoragePath)
                .HasColumnType("NVARCHAR(500)")
                .IsRequired();

            c.Property(mf => mf.FileType)
                .HasColumnType("NVARCHAR(50)")
                .IsRequired();

            c.Property(mf => mf.FileSize)
                .HasColumnType("BIGINT")
                .IsRequired();

            c.HasIndex(f => f.SectionId);

            c.HasIndex(f => new { f.SectionId, f.OrderIndex }).IsUnique(); 
        });
    }
}
