using auth_service.Application.Abstractions.Caching;
using MediatR;

namespace auth_service.Application.Features.Auth.Commands.Logout
{
    public class LogoutHandler : IRequestHandler<LogoutCommand, Task>
    {
        private readonly IRefreshTokenStore _refreshTokenStore;

        public LogoutHandler(IRefreshTokenStore refreshTokenStore)
        {
            _refreshTokenStore = refreshTokenStore;
        }

        public async Task<Task> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            await _refreshTokenStore.RevokeRefreshTokenAsync(request.UserId, request.SessionType);
            return Task.CompletedTask;
        }
    }
}