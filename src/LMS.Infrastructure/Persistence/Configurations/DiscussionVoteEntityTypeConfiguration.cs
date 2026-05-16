using LMS.Domain.Entities.CourseDiscussion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public class DiscussionVoteEntityTypeConfiguration : IEntityTypeConfiguration<DiscussionVote>
{
    public void Configure(EntityTypeBuilder<DiscussionVote> builder)
    {
        builder.ToTable("DiscussionVotes");

        builder.HasKey(dv => new { dv.DiscussionId, dv.StudentId });

        builder.HasOne(dv => dv.Discussion)
            .WithMany(d => d.Votes)
            .HasForeignKey(dv => dv.DiscussionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(dv => dv.Student)
            .WithMany(s => s.DiscussionVotes)
            .HasForeignKey(dv => dv.StudentId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
