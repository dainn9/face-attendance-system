using attendance_service.Domain.Aggregates.Course;
using attendance_service.Domain.Aggregates.CourseSection;
using Microsoft.EntityFrameworkCore;

namespace attendance_service.Infrastructure.Persistence
{
    public class AttendanceDbContext : DbContext
    {
        public AttendanceDbContext(DbContextOptions<AttendanceDbContext> options) : base(options) { }

        public DbSet<Course> Courses => Set<Course>();
        public DbSet<CourseSession> Sessions => Set<CourseSession>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AttendanceDbContext).Assembly);
        }
    }
}