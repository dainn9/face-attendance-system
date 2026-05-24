using user_service.Application.Contracts;
using user_service.Domain.Aggregates.User;

namespace user_service.Application.Abstractions.Persistence
{
    public interface IUserRepository
    {
        void Add(User user);
        Task<UserDto?> GetProfileByIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<bool> ExistsByIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}