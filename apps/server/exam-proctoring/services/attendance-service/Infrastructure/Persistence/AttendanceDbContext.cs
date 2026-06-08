using attendance_service.Domain.Aggregates.Subject;
using attendance_service.Domain.Aggregates.CourseSection;
using Microsoft.EntityFrameworkCore;
using attendance_service.Domain.Aggregates.AttendanceSession;

namespace attendance_service.Infrastructure.Persistence
{
    public class AttendanceDbContext : DbContext
    {
        public AttendanceDbContext(DbContextOptions<AttendanceDbContext> options) : base(options) { }

        public DbSet<Subject> Subjects => Set<Subject>();
        public DbSet<CourseSection> CourseSections => Set<CourseSection>();
        public DbSet<AttendanceSession> AttendanceSessions => Set<AttendanceSession>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AttendanceDbContext).Assembly);
        }
    }
}