using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Domin.Entities.EntitiesConfiguration;

public class RefreshTokenEntityTypeConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(r => r.Id);

        builder.HasIndex(r => r.Token).IsUnique();

        builder.HasOne(r => r.User)
               .WithMany()
               .HasForeignKey(r => r.UserId);
    }
}
