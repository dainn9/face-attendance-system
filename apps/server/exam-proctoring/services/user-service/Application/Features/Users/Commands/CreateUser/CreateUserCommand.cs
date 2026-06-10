using MediatR;
using SharedKernel.Core.Enums;

namespace user_service.Application.Features.Users.Commands.CreateUser
{
    public record CreateUserCommand(
        Guid UserId,
        string? UserCode,
        string FullName,
        Gender Gender,
        DateOnly DateOfBirth,
        string Email,
        UserRole Role,
        Guid? FacultyId,
        Guid? MajorId
    ) : IRequest;
}