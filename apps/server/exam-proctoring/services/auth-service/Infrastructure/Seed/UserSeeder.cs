using auth_service.Application.Abstractions.Security;
using auth_service.Application.Abstractions.Seed;
using auth_service.Domain.Aggregates.User;
using auth_service.Domain.Enum;
using auth_service.Domain.ValueObjects;
using auth_service.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace auth_service.Infrastructure.Seed
{
    public class UserSeeder : IUserSeeder
    {
        private readonly AuthDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public UserSeeder(AuthDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task SeedAsync()
        {
            if (await _context.Users.AnyAsync(x => x.Role == UserRole.Admin))
                return;

            var now = DateTime.UtcNow;
            var adminUser = User.Create(
                email: Email.Create("admin@system.vn"),
                passwordHash: PasswordHash.Create(_passwordHasher.Hash("Admin@123")),
                role: UserRole.Admin,
                now: now
            );

            adminUser.UpdateActiveStatus(true, now);

            _context.Users.Add(adminUser);
            await _context.SaveChangesAsync();
        }
    }
}
