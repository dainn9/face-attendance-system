using attendance_service.Domain.Aggregates.AttendanceSession;
using Microsoft.EntityFrameworkCore;

namespace attendance_service.Infrastructure.Persistence.Configurations
{
    public class AttendanceSessionConfiguration : IEntityTypeConfiguration<AttendanceSession>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<AttendanceSession> builder)
        {
            builder.ToTable("attendance_sessions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).IsRequired();

            builder.Property(x => x.CourseSectionId).IsRequired();
            builder.Property(x => x.Date).IsRequired();
            builder.Property(x => x.StartTime).IsRequired();
            builder.Property(x => x.EndTime);
            builder.Property(x => x.Status).IsRequired().HasConversion<string>();

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder.OwnsMany(e => e.Records, a =>
            {
                a.ToTable("attendance_records");

                a.WithOwner().HasForeignKey("AttendanceSessionId");

                a.HasKey(x => new { x.AttendanceSessionId, x.Id });

                a.Property(x => x.Id)
                    .ValueGeneratedNever();

                a.Property(x => x.AttendanceSessionId).IsRequired();
                a.Property(x => x.StudentId).IsRequired();
                a.Property(x => x.Status).IsRequired().HasConversion<string>();
                a.Property(x => x.CheckedInAt);
                a.Property(x => x.Confidence);

                a.HasIndex(x => x.StudentId);

                a.HasIndex(x => new { x.AttendanceSessionId, x.StudentId })
                    .IsUnique();
            });
        }
    }
}