using System.Security.Claims;
using BuildingBlocks.Exceptions;

namespace BuildingBlocks.Extensions
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
    }
}