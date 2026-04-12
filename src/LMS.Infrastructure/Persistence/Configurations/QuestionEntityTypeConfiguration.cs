using LMS.Domain.Entities.Quizzes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public class QuestionEntityTypeConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.QuestionText)
            .HasColumnType("NVARCHAR(2000)")
            .IsRequired();

        builder.Property(q => q.Type)
            .HasConversion<string>()
            .HasColumnType("VARCHAR(10)")
            .IsRequired();

        builder.Property(q => q.Mark)
            .HasColumnType("FLOAT")
            .IsRequired();

        builder.Property(q => q.Order)
            .HasColumnType("INT")
            .IsRequired();

        builder.Property(q => q.Instructions)
            .HasColumnType("NVARCHAR(2000)")
            .IsRequired(false);

        builder.Property(q => q.Explanation)
            .HasColumnType("NVARCHAR(2000)")
            .IsRequired(false);

        builder.Property(q => q.QuizId)
            .HasColumnType("UNIQUEIDENTIFIER")
            .IsRequired();

        builder.HasOne(q => q.Quiz)
            .WithMany(qz => qz.Questions)
            .HasForeignKey(q => q.QuizId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(q => q.Answers)
            .WithOne(a => a.Question)
            .HasForeignKey(a => a.QuestionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}

