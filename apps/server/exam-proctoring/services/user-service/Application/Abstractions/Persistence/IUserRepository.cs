using user_service.Domain.Aggregates.User;

namespace user_service.Application.Abstractions.Persistence
{
    public interface IUserRepository
    {
        void Add(User user);
        Task<bool> ExistsByIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<bool> ExistsByUserCodeAsync(string userCode, CancellationToken cancellationToken);
    }
}