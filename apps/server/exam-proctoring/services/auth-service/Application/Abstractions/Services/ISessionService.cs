using auth_service.Application.Contracts;
using auth_service.Domain.Aggregates.User;
using auth_service.Domain.Enum;

namespace auth_service.Application.Abstractions.Services
{
    public interface ISessionService
    {
        Task<AuthResponse> CreateSessionAsync(User user, SessionType sessionType, CancellationToken ct);

    }
}