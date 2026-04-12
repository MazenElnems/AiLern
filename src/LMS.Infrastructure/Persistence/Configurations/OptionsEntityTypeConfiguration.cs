using LMS.Domain.Entities.Quizzes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

internal class OptionsEntityTypeConfiguration : IEntityTypeConfiguration<Option>
{
    public void Configure(EntityTypeBuilder<Option> builder)
    {
        builder.ToTable("Options");

        builder.HasKey(opt => opt.OptionId);    

        builder.Property(opt => opt.OptionNumber)
            .ValueGeneratedNever();

        builder.Property(opt => opt.OptionId)
            .HasDefaultValueSql("NEWID()");

        builder.Property(opt => opt.OptionText)
            .HasColumnType("NVARCHAR(500)")
            .IsRequired();

        builder.Property(opt => opt.IsCorrect)
            .HasColumnType("BIT")
            .IsRequired();

        builder.HasOne(opt => opt.Question)
            .WithMany(q => q.Options)
            .HasForeignKey(opt => opt.QuestionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
