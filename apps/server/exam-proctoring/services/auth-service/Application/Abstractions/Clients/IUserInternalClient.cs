using SharedKernel.Core.Enums;

namespace auth_service.Application.Abstractions.Clients
{
    public interface IUserInternalClient
    {
        Task CreateUserAsync(Guid userId, string fullName, Gender gender, DateOnly dateOfBirth, string email, CancellationToken cancellationToken = default);
    }
}
