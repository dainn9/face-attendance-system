using auth_service.Domain.Aggregates.User;
using auth_service.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace auth_service.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Email)
                .HasConversion(e => e.Value, v => Email.Create(v))
                .IsRequired()
                .HasMaxLength(500);

            builder.HasIndex(x => x.Email).IsUnique();

            builder.Property(x => x.PasswordHash)
                   .HasConversion(p => p.Value, v => PasswordHash.Create(v))
                   .HasMaxLength(500);

            builder.Property(x => x.Role)
                .HasConversion<string>()  // lưu "Admin", "Proctor", "Student"
                .IsRequired();

            builder.Property(x => x.IsActive).IsRequired();

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
        }
    }
}