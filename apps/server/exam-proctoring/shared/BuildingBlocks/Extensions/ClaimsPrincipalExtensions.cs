using System.Security.Claims;
using BuildingBlocks.Exceptions;
using SharedKernel.Core.Enums;

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

        public static UserRole GetUserRole(this ClaimsPrincipal user)
        {
            var role = user.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrWhiteSpace(role))
                throw new UnauthorizedException(
                    "User role not found.",
                    ErrorCodes.Unauthorized);

            if (!Enum.TryParse<UserRole>(role, out var userRole))
                throw new UnauthorizedException(
                    "Invalid user role.",
                    ErrorCodes.Unauthorized);

            return userRole;
        }
    }
}