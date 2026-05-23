using auth_service.Application.Abstractions.Services;
using auth_service.Application.Contracts;
using auth_service.Domain.Enum;
using BuildingBlocks.Exceptions;
using MediatR;

namespace auth_service.Application.Features.Auth.Commands.Login
{
    public class LoginHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly IAuthService _authService;
        private readonly ISessionService _sessionService;

        public LoginHandler(IAuthService authService, ISessionService sessionService)
        {
            _authService = authService;
            _sessionService = sessionService;
        }

        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _authService.AuthenticateAsync(request.Email, request.Password, cancellationToken);

            var sesionType = user.Role switch
            {
                UserRole.Admin => SessionType.Admin,
                UserRole.Lecturer => SessionType.Lecturer,
                UserRole.Student => SessionType.Student,
                _ => throw new UnauthorizedException("User role is not recognized.", ErrorCodes.Unauthorized)
            };


            return await _sessionService.CreateSessionAsync(user, sesionType, cancellationToken);
        }
    }
}