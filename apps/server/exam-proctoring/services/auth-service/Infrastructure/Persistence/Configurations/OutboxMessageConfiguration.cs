using auth_service.Domain.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace auth_service.Infrastructure.Persistence.Configurations
{
    public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Payload)
                .HasColumnType("longtext")
                .IsRequired();

            builder.Property(x => x.Error)
                .HasMaxLength(2000);

            builder.HasIndex(x => new { x.ProcessedAt, x.NextRetryAt });
        }
    }
}