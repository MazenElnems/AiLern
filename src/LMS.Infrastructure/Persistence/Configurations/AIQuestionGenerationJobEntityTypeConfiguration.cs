using LMS.Domain.Entities.Quizzes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public class AIQuestionGenerationJobEntityTypeConfiguration : IEntityTypeConfiguration<AIQuestionGenerationJob>
{
    public void Configure(EntityTypeBuilder<AIQuestionGenerationJob> builder)
    {
        builder.HasOne(x => x.Quiz)
            .WithMany(j => j.QuestionGenerationJobs)
            .HasForeignKey(x => x.QuizId);

        builder.Property(x => x.HangfireJobId).IsRequired();
    }
}
