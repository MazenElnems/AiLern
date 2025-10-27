using LMS.Core.Domain.Entities;
using LMS.Core.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Shared.Domain.Entities.EntitiesConfiguration
{
    public class CourseEntityTypeConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder
                .ToTable("Courses");

            builder
                .HasKey(c => c.Id);

            builder
                .HasIndex(c => c.Code)
                .IsUnique();

            builder
                .HasIndex(c => c.Name)
                .IsUnique();

            builder.Property(c => c.Code)
                .HasColumnType("VARCHAR(10)")
                .IsRequired();

            builder.Property(c => c.Name)
                .HasColumnType("VARCHAR(50)")
                .IsRequired();

            builder
                .Property(c => c.Description)
                .HasColumnType("NVARCHAR(MAX)")
                .IsRequired(false);

            builder.HasOne(c => c.Instructor)
                .WithMany(i => i.Courses)
                .HasForeignKey(c => c.InstructorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(c => c.CreatedAt)
                .HasColumnType("DATETIME2")
                .IsRequired();

            builder.Property(c => c.ApprovedDate)
                .HasColumnType("DATETIME2")
                .IsRequired(false);

            builder.Property(c => c.CourseStatus)
                .HasConversion<int>()
                .HasDefaultValue(CourseStatus.Pending);

            builder.HasOne(c => c.Admin)
                .WithMany()
                .HasForeignKey(c => c.Approvedby)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(c => c.Section)
                .WithOne()
                .HasForeignKey<Course>(c => c.SectionCourseId)
                .IsRequired(false).OnDelete(DeleteBehavior.NoAction);

        }
    }
}
