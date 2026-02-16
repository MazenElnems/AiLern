using LMS.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Domain.Entities.EntitiesConfiguration;

public class ApplicationUserEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasDiscriminator<string>("Role")
        .HasValue<Admin>("Admin")
        .HasValue<Student>("Student")
        .HasValue<Instructor>("Instructor");
    }
}
