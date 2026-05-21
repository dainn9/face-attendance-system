using Microsoft.EntityFrameworkCore;
using user_service.Application.Abstractions.Persistence;
using user_service.Application.Contracts;
using user_service.Domain.Aggregates.User;

namespace user_service.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext Context) => _context = Context;

        public async Task AddAsync(User user, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public Task<UserDto?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
        => _context.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new UserDto(
                u.Id,
                u.FullName,
                u.Gender,
                u.DateOfBirth,
                u.Email
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }
}