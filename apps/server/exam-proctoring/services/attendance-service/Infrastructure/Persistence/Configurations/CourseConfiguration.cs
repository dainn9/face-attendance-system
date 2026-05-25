using attendance_service.Domain.Aggregates.Course;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace attendance_service.Infrastructure.Persistence.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.ToTable("courses");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).IsRequired();

            builder.Property(x => x.Code)
                    .IsRequired()
                    .HasMaxLength(20);

            builder.HasIndex(x => x.Code).IsUnique();

            builder.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);

            builder.Property(x => x.Credits)
                    .IsRequired();

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
        }
    }
}