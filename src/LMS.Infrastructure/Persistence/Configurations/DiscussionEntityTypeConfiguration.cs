using LMS.Domain.Entities.CourseDiscussion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public class DiscussionEntityTypeConfiguration : IEntityTypeConfiguration<Discussion>
{
    public void Configure(EntityTypeBuilder<Discussion> builder)
    {
        builder.ToTable("Discussions");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
            .ValueGeneratedOnAdd();

        builder.Property(d => d.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(d => d.Content)
            .IsRequired();

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.IsAnswered)
            .HasDefaultValue(false);

        builder.Property(d => d.IsPinned)
            .HasDefaultValue(false);

        // Relationships

        // One Course -> Many Discussions
        builder.HasOne(d => d.Course)
            .WithMany(c => c.Discussions)
            .HasForeignKey(d => d.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // One Student (Author) -> Many Discussions
        builder.HasOne(d => d.Student)
            .WithMany(s => s.Discussions)
            .HasForeignKey(d => d.StudentId)
            .OnDelete(DeleteBehavior.NoAction); // Avoid multiple cascade paths

        // Many Students (Voters) <-> Many Discussions
        builder.HasMany(d => d.Votes)
            .WithOne(v => v.Discussion)
            .HasForeignKey(v => v.DiscussionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(d => d.CourseId);
        builder.HasIndex(d => d.StudentId);
    }
}
