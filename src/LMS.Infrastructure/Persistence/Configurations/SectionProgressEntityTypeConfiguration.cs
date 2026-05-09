using LMS.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

internal class SectionProgressEntityTypeConfiguration : IEntityTypeConfiguration<SectionProgress>
{
    public void Configure(EntityTypeBuilder<SectionProgress> builder)
    {
        builder
            .ToTable("StudentSectionProgress");

        builder
            .HasKey(s => new {s.StudentId, s.SectionId});

        builder
            .Property(s => s.IsCompleted)
            .IsRequired();

        builder
            .HasOne(s => s.Student)
            .WithMany(s => s.SectionProgresses)
            .HasForeignKey(s => s.StudentId)
            .IsRequired();
        
        builder
            .HasOne(s => s.Section)
            .WithMany(s => s.SectionProgresses)
            .HasForeignKey(s => s.SectionId)
            .IsRequired();
    }
}
