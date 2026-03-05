using LMS.Domain.Entities.Quizzes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public class QuizEntityTypeConfiguration : IEntityTypeConfiguration<Quiz>
{
    public void Configure(EntityTypeBuilder<Quiz> builder)
    {
        builder.ToTable("Quizzes");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.Title)
            .HasColumnType("NVARCHAR(200)")
            .IsRequired();

        builder.Property(q => q.Description)
            .HasColumnType("NVARCHAR(2000)")
            .IsRequired(false);

        builder.Property(q => q.AvailableFrom)
            .HasColumnType("DATETIME2")
            .IsRequired();

        builder.Property(q => q.AvailableUntil)
            .HasColumnType("DATETIME2")
            .IsRequired();

        builder.Property(q => q.MaximumAttempts)
            .HasColumnType("INT")
            .IsRequired();

        builder.Property(q => q.ShowResultOnClose)
            .HasColumnType("BIT")
            .IsRequired();

        builder.Property(q => q.CreatedAt)
            .HasColumnType("DATETIME2")
            .IsRequired();

        builder.Property(q => q.UpdatedAt)
            .HasColumnType("DATETIME2")
            .IsRequired(false);

        builder.Property(q => q.Status)
            .HasConversion<string>()
            .HasColumnType("VARCHAR(10)")
            .IsRequired();

        builder.HasOne(q => q.Course)
            .WithMany(c => c.Quizzes)
            .HasForeignKey(q => q.CourseId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(q => q.Questions)
            .WithOne(qu => qu.Quiz)
            .HasForeignKey(qu => qu.QuizId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
