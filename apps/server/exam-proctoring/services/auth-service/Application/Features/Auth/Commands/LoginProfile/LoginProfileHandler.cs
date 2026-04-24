using auth_service.Application.Abstractions.Services;
using auth_service.Application.Contracts;
using auth_service.Domain.Enum;
using MediatR;

namespace auth_service.Application.Features.Auth.Commands.LoginProfile
{
    public class LoginProfileHandler : IRequestHandler<LoginProfileCommand, AuthResponse>
    {
        private readonly IAuthService _authService;
        private readonly ISessionService _sessionService;

        public LoginProfileHandler(IAuthService authService, ISessionService sessionService)
        {
            _authService = authService;
            _sessionService = sessionService;
        }

        public async Task<AuthResponse> Handle(LoginProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _authService.AuthenticateAsync(request.Email, request.Password, cancellationToken);
            return await _sessionService.CreateSessionAsync(user, SessionType.Profile, cancellationToken);
        }
    }
}