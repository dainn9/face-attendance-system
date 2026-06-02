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

        public void Add(User user)
        => _context.Users.Add(user);

        public void Remove(User user)
        => _context.Users.Remove(user);

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

        public Task<User?> GetTrackedByIdAsync(Guid id, CancellationToken ct = default)
            => _context.Users
                .FirstOrDefaultAsync(u => u.Id == id, ct);

        public Task<Dictionary<Guid, bool>> GetStatusByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
        => _context.Users
            .AsNoTracking()
            .Where(u => ids.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.IsActive, ct);
    }
}