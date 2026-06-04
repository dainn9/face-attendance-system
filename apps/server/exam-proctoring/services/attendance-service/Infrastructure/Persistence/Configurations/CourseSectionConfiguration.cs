using attendance_service.Domain.Aggregates.CourseSection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace attendance_service.Infrastructure.Persistence.Configurations
{
    public class CourseSectionConfiguration : IEntityTypeConfiguration<CourseSection>
    {
        public void Configure(EntityTypeBuilder<CourseSection> builder)
        {
            builder.ToTable("course_sections");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).IsRequired();

            builder.Property(x => x.SubjectId)
                    .IsRequired();

            builder.Property(x => x.CourseSectionCode)
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

            builder.Property(x => x.MaxCapacity).IsRequired();
            builder.Property(x => x.IsActive).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder.OwnsMany(e => e.Enrollments, a =>
            {
                a.ToTable("enrollments");

                a.WithOwner().HasForeignKey("CourseSectionId");

                a.HasKey(x => new { x.CourseSectionId, x.Id });

                a.Property(x => x.Id)
                    .ValueGeneratedNever();

                a.HasIndex(x => x.StudentId);

                a.Property(e => e.CourseSectionId).IsRequired();
                a.Property(e => e.StudentId).IsRequired();
                a.Property(e => e.EnrolledAt).IsRequired();


                a.HasIndex(e => new { e.CourseSectionId, e.StudentId })
                    .IsUnique()
                    .HasDatabaseName("IX_Enrollment_CourseSectionId_StudentId");
            });

            builder.OwnsMany(e => e.Schedules, a =>
            {
                a.ToTable("schedules");

                a.WithOwner().HasForeignKey("CourseSectionId");

                a.HasKey(x => new { x.CourseSectionId, x.Id });

                a.Property(x => x.Id)
                    .ValueGeneratedNever();

                a.Property(e => e.CourseSectionId).IsRequired();
                a.Property(e => e.DayOfWeek).IsRequired();
                a.Property(e => e.StartTime).IsRequired();
                a.Property(e => e.EndTime).IsRequired();
                a.Property(e => e.Room)
                    .IsRequired()
                    .HasMaxLength(50);

                a.HasIndex(e => new { e.CourseSectionId, e.DayOfWeek, e.StartTime })
                    .IsUnique()
                    .HasDatabaseName("IX_Schedule_CourseSectionId_DayOfWeek_StartTime");
            });
        }
    }
}