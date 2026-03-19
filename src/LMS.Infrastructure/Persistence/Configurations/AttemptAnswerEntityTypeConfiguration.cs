using LMS.Domain.Entities.Quizzes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public class AttemptAnswerEntityTypeConfiguration : IEntityTypeConfiguration<AttemptAnswer>
{
    public void Configure(EntityTypeBuilder<AttemptAnswer> builder)
    {
        builder.ToTable("AttemptAnswers");

        builder.HasKey(a => new { a.AttemptId, a.QuestionId });

        builder.Property(a => a.AttemptId)
            .HasColumnType("UNIQUEIDENTIFIER")
            .IsRequired();

        builder.Property(a => a.QuestionId)
            .HasColumnType("UNIQUEIDENTIFIER")
            .IsRequired();

        builder.Property(a => a.BooleanAnswer)
            .HasColumnType("VARCHAR(5)")
            .IsRequired(false);

        builder.Property(a => a.WrittenAnswer)
            .HasColumnType("NVARCHAR(MAX)")
            .IsRequired(false);

        builder.Property(a => a.OptionNumber)
            .HasColumnType("INT")
            .IsRequired(false);

        builder.Property(a => a.Mark)
            .HasColumnType("FLOAT")
            .IsRequired(false);

        builder.Property(a => a.Feedback)
            .HasColumnType("NVARCHAR(MAX)")
            .IsRequired(false);

        builder.HasOne(a => a.Attempt)
            .WithMany(a => a.AttemptAnswers)
            .HasForeignKey(a => a.AttemptId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Question)
            .WithMany(q => q.AttemptAnswers)
            .HasForeignKey(a => a.QuestionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
