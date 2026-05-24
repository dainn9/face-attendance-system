using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using user_service.Domain.Aggregates.User;

namespace user_service.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.FullName)
                    .IsRequired()
                    .HasMaxLength(100);

            builder.Property(x => x.Gender)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(20);

            builder.Property(x => x.DateOfBirth)
                    .IsRequired();

            builder.Property(x => x.Email)
                    .IsRequired()
                    .HasMaxLength(500);

            builder.HasIndex(x => x.Email).IsUnique();

            builder.Property(x => x.Role)
                    .HasConversion<string>()
                    .IsRequired();

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder.OwnsOne(x => x.StudentProfile, sp =>
            {
                sp.ToTable("student_profiles");

                sp.WithOwner().HasForeignKey(x => x.UserId);

                sp.HasKey(x => x.UserId);

                sp.Property(x => x.StudentCode)
                    .IsRequired()
                    .HasMaxLength(20);

                sp.HasIndex(x => x.StudentCode)
                    .IsUnique();

                sp.Property(x => x.FacultyCode)
                    .IsRequired()
                    .HasMaxLength(20);

                sp.Property(x => x.MajorCode)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            builder.OwnsOne(x => x.LecturerProfile, lp =>
            {
                lp.ToTable("lecturer_profiles");

                lp.WithOwner().HasForeignKey(x => x.UserId);

                lp.HasKey(x => x.UserId);

                lp.Property(x => x.LecturerCode)
                    .IsRequired()
                    .HasMaxLength(20);

                lp.HasIndex(x => x.LecturerCode)
                    .IsUnique();

                lp.Property(x => x.FacultyCode)
                    .IsRequired()
                    .HasMaxLength(20);
            });
        }
    }
}