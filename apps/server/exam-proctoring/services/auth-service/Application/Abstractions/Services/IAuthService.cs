using auth_service.Domain.Aggregates.User;

namespace auth_service.Application.Abstractions.Services
{
    public interface IAuthService
    {
        // Task<AuthResponse> LoginAsync(string email, string password, SessionType sessionType, CancellationToken ct);
        Task<User> AuthenticateAsync(string email, string password, CancellationToken ct);
    }
}