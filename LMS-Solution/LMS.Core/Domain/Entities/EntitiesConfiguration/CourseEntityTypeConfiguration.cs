using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Domain.Entities.EntitiesConfiguration
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

            builder
                .Property(c => c.Description)
                .HasColumnType("NVARCHAR(MAX)")
                .IsRequired(false);

        }
    }
}
