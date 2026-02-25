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
            .HasColumnType("NVARCHAR(MAX)")
            .IsRequired();

        builder.Property(q => q.QuestionType)
            .HasConversion<string>()
            .HasColumnType("NVARCHAR(20)")
            .IsRequired();

        builder.Property(q => q.Mark)
            .HasColumnType("FLOAT")
            .IsRequired();

        builder.Property(q => q.Order)
            .HasColumnType("INT")
            .IsRequired();

        builder.Property(q => q.Instructions)
            .HasColumnType("NVARCHAR(MAX)")
            .IsRequired();

        builder.Property(q => q.QuizId)
            .HasColumnType("UNIQUEIDENTIFIER")
            .IsRequired();

        builder.HasOne(q => q.Quiz)
            .WithMany(qz => qz.Questions)
            .HasForeignKey(q => q.QuizId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsMany(q => q.Options, o =>
        {
            o.ToTable("QuestionOptions");
            
            o.WithOwner(o => o.Question).HasForeignKey("QuestionId");
            
            o.HasKey(opt => new { opt.OptionNumber, opt.QuestionId });
            
            o.Property(opt => opt.OptionText)
                .HasColumnType("NVARCHAR(MAX)")
                .IsRequired();
           
            o.Property(opt => opt.IsCorrect)
                .HasColumnType("BIT")
                .IsRequired();
        });
    }
}

