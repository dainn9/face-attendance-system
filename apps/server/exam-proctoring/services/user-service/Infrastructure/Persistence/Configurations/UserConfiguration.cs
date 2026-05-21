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

            builder.Property(x => x.Id).IsRequired();
            builder.HasIndex(x => x.Id);

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

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
        }
    }
}