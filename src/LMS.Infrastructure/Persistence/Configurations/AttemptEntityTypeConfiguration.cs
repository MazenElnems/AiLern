using LMS.Domain.Entities.Quizzes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public class AttemptEntityTypeConfiguration : IEntityTypeConfiguration<Attempt>
{
    public void Configure(EntityTypeBuilder<Attempt> builder)
    {
        builder.ToTable("Attempts");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedNever();

        builder.Property(a => a.StudentId)
            .HasColumnType("INT")
            .IsRequired();

        builder.Property(a => a.QuizId)
            .HasColumnType("UNIQUEIDENTIFIER")
            .IsRequired();

        builder.Property(a => a.AttemptEndTime)
            .HasColumnType("DATETIME2")
            .IsRequired();

        builder.Property(a => a.StartAt)
            .HasColumnType("DATETIME2")
            .IsRequired();

        builder.Property(a => a.SubmittedAt)
            .HasColumnType("DATETIME2")
            .IsRequired(false);

        builder.Property(a => a.SavedAt)
            .HasColumnType("DATETIME2")
            .IsRequired(false);

        builder.Property(a => a.AttemptNumber)
            .HasColumnType("INT")
            .IsRequired();

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasColumnType("VARCHAR(10)")
            .IsRequired();

        builder.Property(a => a.ShuffledQuestionIds)
            .HasColumnType("NVARCHAR(MAX)")
            .IsRequired(false);

        // to avoid race condition and avoid multiple start attempts
        builder.HasIndex(a => new { a.QuizId, a.StudentId, a.AttemptNumber })
            .IsUnique();

        builder.HasIndex(a => a.QuizId);

        builder.HasOne(a => a.Student)
            .WithMany(s => s.Attempts)
            .HasForeignKey(a => a.StudentId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Quiz)
            .WithMany(q => q.Attempts)
            .HasForeignKey(a => a.QuizId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
