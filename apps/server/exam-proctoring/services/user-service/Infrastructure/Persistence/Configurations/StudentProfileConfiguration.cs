using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using user_service.Domain.Aggregates.User;

namespace user_service.Infrastructure.Persistence.Configurations
{
    public class StudentProfileConfiguration : IEntityTypeConfiguration<StudentProfile>
    {
        public void Configure(EntityTypeBuilder<StudentProfile> builder)
        {
            builder.ToTable("student_profiles");

            builder.HasKey(x => x.UserId);

            builder.Property(x => x.UserId).IsRequired();

            builder.Property(x => x.StudentCode)
                    .IsRequired()
                    .HasMaxLength(20);

            builder.HasIndex(x => x.StudentCode)
                    .IsUnique();

            builder.Property(x => x.FacultyCode)
                    .IsRequired()
                    .HasMaxLength(20);

            builder.Property(x => x.MajorCode)
                    .IsRequired()
                    .HasMaxLength(20);

            builder.HasOne(x => x.User)
                    .WithOne(x => x.StudentProfile)
                    .HasForeignKey<StudentProfile>(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}