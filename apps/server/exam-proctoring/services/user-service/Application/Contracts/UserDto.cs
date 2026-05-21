using user_service.Domain.Enum;

namespace user_service.Application.Contracts
{
    public sealed record UserDto(Guid UserId, string FullName, Gender Gender, DateOnly DateOfBirth, string Email);
}