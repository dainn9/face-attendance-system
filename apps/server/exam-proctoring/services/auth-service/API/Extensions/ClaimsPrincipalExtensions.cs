using System.Security.Claims;
using auth_service.Domain.Enum;
using BuildingBlocks.Exceptions;

namespace auth_service.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var sub = user.FindFirst("sub")?.Value
                    ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (sub is null)
                throw new UnauthorizedException("User not identified.", ErrorCodes.Unauthorized);

            if (!Guid.TryParse(sub, out var userId))
                throw new UnauthorizedException("User not identified.", ErrorCodes.Unauthorized);

            return userId;
        }

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