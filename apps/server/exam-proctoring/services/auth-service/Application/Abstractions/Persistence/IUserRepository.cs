using auth_service.Domain.Aggregates.User;
using auth_service.Domain.ValueObjects;

namespace auth_service.Application.Abstractions.Persistence
{
    public interface IUserRepository
    {
        Task AddAsync(User user, CancellationToken ct = default);
        Task UpdateAsync(User user, CancellationToken ct = default);
        Task<User?> GetByEmailAsync(Email email, CancellationToken ct = default);
        Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct = default);
        Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    }
}