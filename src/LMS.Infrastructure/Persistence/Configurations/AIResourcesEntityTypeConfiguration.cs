using LMS.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public class AIResourcesEntityTypeConfiguration : IEntityTypeConfiguration<AIResource>
{
    public void Configure(EntityTypeBuilder<AIResource> builder)
    {
        builder.ToTable("AIResources");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.FileSize)
            .IsRequired();

        builder.Property(a => a.FileType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.UploadStatus)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(a => a.StoragePath)
            .IsRequired()
            .HasMaxLength(500);

        // Relationship: One Course, Many AIResources
        builder.HasOne(a => a.Course)
            .WithMany(c => c.AIResources)
            .HasForeignKey(a => a.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(a => a.CourseId)
            .HasDatabaseName("IX_AIResources_CourseId");

        builder.HasIndex(a => a.UploadStatus)
            .HasDatabaseName("IX_AIResources_Status");
    }
}
