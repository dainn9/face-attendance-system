using attendance_service.Domain.Aggregates.CourseSection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace attendance_service.Infrastructure.Persistence.Configurations
{
    public class CourseSessionConfiguration : IEntityTypeConfiguration<CourseSession>
    {
        public void Configure(EntityTypeBuilder<CourseSession> builder)
        {
            builder.ToTable("course_sessions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).IsRequired();

            builder.Property(x => x.CourseId)
                    .IsRequired();

            builder.Property(x => x.SectionCode)
                    .IsRequired()
                    .HasMaxLength(20);

            builder.Property(x => x.Semester)
                    .IsRequired()
                    .HasMaxLength(20);

            builder.Property(x => x.AcademicYear)
                    .IsRequired()
                    .HasMaxLength(20);

            builder.Property(x => x.LecturerId)
                    .IsRequired();

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder.OwnsMany(e => e.Enrollments, a =>
            {
                a.ToTable("enrollments");
                a.HasKey("CourseSessionId", "StudentId");
                a.Property(e => e.StudentId).IsRequired();
                a.Property(e => e.EnrolledAt).IsRequired();
            });
        }
    }
}