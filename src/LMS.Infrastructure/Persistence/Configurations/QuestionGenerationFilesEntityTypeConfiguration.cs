using LMS.Domain.Entities.Quizzes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public class QuestionGenerationFilesEntityTypeConfiguration : IEntityTypeConfiguration<QuestionGenerationFiles>
{
    public void Configure(EntityTypeBuilder<QuestionGenerationFiles> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();
    }
}
