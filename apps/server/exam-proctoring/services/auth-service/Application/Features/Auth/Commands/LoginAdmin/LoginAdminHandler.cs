using auth_service.Application.Abstractions.Services;
using auth_service.Application.Contracts;
using auth_service.Domain.Enum;
using BuildingBlocks.Exceptions;
using MediatR;

namespace auth_service.Application.Features.Auth.Commands.LoginAdmin
{
    public class LoginAdminHandler : IRequestHandler<LoginAdminCommand, AuthResponse>
    {
        private readonly IAuthService _authService;
        private readonly ISessionService _sessionService;

        public LoginAdminHandler(IAuthService authService, ISessionService sessionService)
        {
            _authService = authService;
            _sessionService = sessionService;
        }

        public async Task<AuthResponse> Handle(LoginAdminCommand request, CancellationToken ct)
        {
            var user = await _authService.AuthenticateAsync(request.Email, request.Password, ct);

            if (user.Role != UserRole.Admin)
                throw new UnauthorizedException("User does not have admin privileges.", ErrorCodes.Unauthorized);

            return await _sessionService.CreateSessionAsync(user, SessionType.Admin, ct);
        }
    }
}