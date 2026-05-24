using Microsoft.EntityFrameworkCore;
using user_service.Domain.Aggregates.User;

namespace user_service.Infrastructure.Persistence.Configurations
{
    public class LecturerProfileConfiguration : IEntityTypeConfiguration<LecturerProfile>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<LecturerProfile> builder)
        {
            builder.ToTable("lecturer_profiles");

            builder.HasKey(x => x.UserId);

            builder.Property(x => x.UserId).IsRequired();

            builder.Property(x => x.LecturerCode)
                    .IsRequired()
                    .HasMaxLength(20);

            builder.HasIndex(x => x.LecturerCode)
                    .IsUnique();

            builder.Property(x => x.FacultyCode)
                    .IsRequired()
                    .HasMaxLength(20);

            builder.HasOne(x => x.User)
                    .WithOne(x => x.LecturerProfile)
                    .HasForeignKey<LecturerProfile>(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}