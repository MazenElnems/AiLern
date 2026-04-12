using LMS.Domain.Entities.Quizzes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public class AnswerEntityTypeConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.ToTable("Answers");

        builder.HasKey(a => new { a.AttemptId, a.QuestionId });

        builder.Property(a => a.AttemptId)
            .HasColumnType("UNIQUEIDENTIFIER")
            .IsRequired();

        builder.Property(a => a.QuestionId)
            .HasColumnType("UNIQUEIDENTIFIER")
            .IsRequired();

        builder.Property(a => a.WrittenAnswer)
            .HasColumnType("NVARCHAR(3000)")
            .IsRequired(false);

        builder.Property(a => a.Mark)
            .HasColumnType("FLOAT")
            .IsRequired(false);

        builder.Property(a => a.Feedback)
            .HasColumnType("NVARCHAR(3000)")
            .IsRequired(false);

        builder.HasOne(a => a.Attempt)
            .WithMany(a => a.Answers)
            .HasForeignKey(a => a.AttemptId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Question)
            .WithMany(q => q.Answers)
            .HasForeignKey(a => a.QuestionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Option)
            .WithMany(q => q.Answers)
            .HasForeignKey(a => a.OptionId)
            .IsRequired(false);
    }
}
