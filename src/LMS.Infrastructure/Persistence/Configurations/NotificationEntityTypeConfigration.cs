using LMS.Domain.Entities.Notification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

internal class NotificationEntityTypeConfigration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder
            .ToTable("Notifications");

        builder
            .HasKey(n => n.NotificationId);

        builder
            .Property(n => n.Type)
            .HasConversion<string>();

        builder
            .Property(n => n.Title)
            .HasColumnType("NVARCHAR(50)");
 
        builder
            .Property(n => n.Message)
            .HasColumnType("NVARCHAR(200)");

        builder
            .Property(n => n.Url)
            .HasColumnType("VARCHAR(100)");

        builder
            .HasMany(n => n.UserNotifications)
            .WithOne(u => u.Notification)
            .HasForeignKey(u => u.NotificationId)
            .IsRequired();
    }
}
