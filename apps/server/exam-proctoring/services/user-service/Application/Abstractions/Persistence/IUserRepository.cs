using user_service.Application.Contracts;
using user_service.Domain.Aggregates.User;

namespace user_service.Application.Abstractions.Persistence
{
    public interface IUserRepository
    {
        Task AddAsync(User user, CancellationToken cancellationToken);
        Task<UserDto?> GetProfileByIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<bool> ExistsByIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}