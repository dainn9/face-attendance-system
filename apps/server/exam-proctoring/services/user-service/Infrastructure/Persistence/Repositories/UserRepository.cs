using Microsoft.EntityFrameworkCore;
using user_service.Application.Abstractions.Persistence;
using user_service.Domain.Aggregates.User;

namespace user_service.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext Context) => _context = Context;

        public void Add(User user)
        => _context.Users.Add(user);

        public Task<bool> ExistsByIdAsync(Guid userId, CancellationToken cancellationToken)
        => _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == userId, cancellationToken);

        public Task<bool> ExistsByUserCodeAsync(string userCode, CancellationToken cancellationToken)
        => _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.UserCode == userCode, cancellationToken);
    }
}