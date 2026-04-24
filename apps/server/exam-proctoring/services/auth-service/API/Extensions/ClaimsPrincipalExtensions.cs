using System.Security.Claims;
using auth_service.Domain.Enum;
using BuildingBlocks.Exceptions;

namespace auth_service.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static SessionType GetSessionType(this ClaimsPrincipal user)
        {
            var value = user.FindFirst("session_type")?.Value;

            if (string.IsNullOrEmpty(value))
                throw new UnauthorizedException("Missing session type.", ErrorCodes.InvalidSessionType);

            if (!Enum.TryParse<SessionType>(value, out var sessionType))
                throw new UnauthorizedException("Invalid session type.", ErrorCodes.InvalidSessionType);

            return sessionType;
        }
    }
}