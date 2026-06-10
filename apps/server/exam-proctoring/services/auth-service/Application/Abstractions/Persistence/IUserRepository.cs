using auth_service.Domain.Aggregates.User;
using auth_service.Domain.ValueObjects;

namespace auth_service.Application.Abstractions.Persistence
{
    public interface IUserRepository
    {
        void Add(User user);
        Task UpdateAsync(User user, CancellationToken ct = default);
        void Remove(User user);
        Task<User?> GetByEmailAsync(Email email, CancellationToken ct = default);
        Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct = default);
        Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<User?> GetTrackedByIdAsync(Guid id, CancellationToken ct = default);
        Task<Dictionary<Guid, bool>> GetStatusByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    }
}