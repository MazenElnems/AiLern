using LMS.Domain.Entities.Courses;
using LMS.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public class ReportEntityTypeConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.ToTable("Reports");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Type)
            .HasConversion<string>()
            .HasColumnType("NVARCHAR(50)")
            .IsRequired();

        builder.Property(r => r.Comment)
            .HasColumnType("NVARCHAR(MAX)")
            .IsRequired(false);

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasColumnType("NVARCHAR(50)")
            .IsRequired();

        builder.Property(r => r.SubmittedAt)
            .HasColumnType("DATETIME2")
            .IsRequired();

        builder.HasOne(r => r.Student)
            .WithMany(s => s.Reports)
            .HasForeignKey(r => r.StudentId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.MaterialFile)
            .WithMany(m => m.Reports)
            .HasForeignKey(r => r.MaterialId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
