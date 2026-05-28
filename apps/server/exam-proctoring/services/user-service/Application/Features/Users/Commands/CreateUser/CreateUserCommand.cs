using MediatR;
using SharedKernel.Core.Enums;

namespace user_service.Application.Features.Users.Commands.CreateUser
{
    public record CreateUserCommand(
        Guid UserId,
        string FullName,
        Gender Gender,
        DateOnly DateOfBirth,
        string Email,
        UserRole Role,
        string? StudentCode,
        string? ClassCode,
        string? FacultyCode
    ) : IRequest;
}