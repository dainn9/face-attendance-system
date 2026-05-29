using Microsoft.EntityFrameworkCore;
using user_service.Domain.Aggregates.Faculty;
using user_service.Domain.Aggregates.User;

namespace user_service.Infrastructure.Persistence
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Faculty> Faculties => Set<Faculty>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserDbContext).Assembly);
        }
    }
}