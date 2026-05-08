using auth_service.Domain.Enum;

namespace auth_service.Application.Contracts
{
    public record RefreshSession(Guid UserId, SessionType SessionType);
}