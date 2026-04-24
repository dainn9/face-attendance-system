using auth_service.Application.Abstractions.Services;
using auth_service.Application.Contracts;
using auth_service.Domain.Enum;
using BuildingBlocks.Exceptions;
using MediatR;

namespace auth_service.Application.Features.Auth.Commands.LoginProctor
{
    public class LoginProctorHandler : IRequestHandler<LoginProctorCommand, AuthResponse>
    {
        private readonly IAuthService _authService;
        private readonly ISessionService _sessionService;

        public LoginProctorHandler(IAuthService authService, ISessionService sessionService)
        {
            _authService = authService;
            _sessionService = sessionService;
        }

        public async Task<AuthResponse> Handle(LoginProctorCommand request, CancellationToken cancellationToken)
        {
            var user = await _authService.AuthenticateAsync(request.Email, request.Password, cancellationToken);

            if (user.Role == UserRole.Student)
                throw new UnauthorizedException("User does not have proctor privileges.", ErrorCodes.Unauthorized);

            return await _sessionService.CreateSessionAsync(user, SessionType.Proctor, cancellationToken);
        }
    }
}