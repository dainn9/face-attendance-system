using Microsoft.EntityFrameworkCore;
using user_service.Domain.Aggregates.Faculty;

namespace user_service.Infrastructure.Persistence.Configurations
{
    public class FacultyConfiguration : IEntityTypeConfiguration<Faculty>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Faculty> builder)
        {
            builder.ToTable("faculties");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);

            builder.Property(x => x.Code)
                    .IsRequired()
                    .HasMaxLength(20);

            builder.HasIndex(x => x.Code).IsUnique();
            builder.HasIndex(x => x.Name).IsUnique();

            builder.OwnsMany(x => x.Majors, m =>
            {
                m.ToTable("majors");

                m.WithOwner().HasForeignKey("FacultyId");

                m.HasKey(x => new { x.FacultyId, x.Id });

                m.Property(x => x.Id)
                    .ValueGeneratedNever();

                m.Property(x => x.FacultyId)
                    .IsRequired();

                m.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                m.Property(x => x.Code)
                    .IsRequired()
                    .HasMaxLength(20);

                m.HasIndex(x => new { x.FacultyId, x.Code })
                    .IsUnique()
                    .HasDatabaseName("IX_Major_FacultyId_Code");

                m.HasIndex(x => new { x.FacultyId, x.Name })
                    .IsUnique()
                    .HasDatabaseName("IX_Major_FacultyId_Name");
            });
        }
    }
}