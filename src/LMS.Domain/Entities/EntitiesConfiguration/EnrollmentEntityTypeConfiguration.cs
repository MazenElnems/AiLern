using LMS.Domain.Common.Enums;
using LMS.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Entities.EntitiesConfiguration
{
    public class EnrollmentEntityTypeConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.HasKey(e => new { e.Course_id, e.Student_id });

            builder.HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.Course_id);

            builder.HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.Student_id);

            builder.Property(e => e.Status)
                .HasConversion<string>()
                .HasDefaultValue(EnrollmentStatus.Pending);
            
            builder.Property(e=>e.Requested_at)
                .HasColumnType("DATETIME2")
                .IsRequired()
                .HasDefaultValueSql("SYSDATETIME()");
        }
    }
}
