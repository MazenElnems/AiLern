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

        builder.HasMany(s => s.MaterialFiles)
            .WithOne(mf => mf.Section)
            .HasForeignKey(mf => mf.SectionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
