using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Domin.Entities.EntitiesConfiguration;

public class ApplicationUserEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.OwnsMany(u => u.RefreshTokens)
                  .HasIndex(r => r.Token);

        builder.HasDiscriminator<string>("Role")
        .HasValue<Admin>("Admin")
        .HasValue<Student>("Student")
        .HasValue<Instructor>("Instructor");
    }
}
