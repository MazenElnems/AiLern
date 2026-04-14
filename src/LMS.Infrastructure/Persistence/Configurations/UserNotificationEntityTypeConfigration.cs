using LMS.Domain.Entities.Notification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

internal class UserNotificationEntityTypeConfigration : IEntityTypeConfiguration<UserNotification>
{
    public void Configure(EntityTypeBuilder<UserNotification> builder)
    {
        builder
            .ToTable("UserNotifications");

        builder
            .HasKey(u => new { u.UserId, u.NotificationId });

        builder
            .Property(u => u.IsRead)
            .HasColumnType("BIT");

        builder
            .HasIndex(u => u.UserId);

        builder
            .HasIndex(u => u.NotificationId);
    }
}
