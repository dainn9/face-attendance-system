using auth_service.Application.Abstractions.Persistence;
using auth_service.Domain.Aggregates.User;
using auth_service.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace auth_service.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _context;

        public UserRepository(AuthDbContext context) => _context = context;

        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            await _context.Users.AddAsync(user, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(User user, CancellationToken ct = default)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync(ct);
        }

        public Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct = default)
            => _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Email == email, ct);

        public Task<User?> GetByEmailAsync(Email email, CancellationToken ct = default)
            => _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email, ct);

        public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id, ct);
    }
}