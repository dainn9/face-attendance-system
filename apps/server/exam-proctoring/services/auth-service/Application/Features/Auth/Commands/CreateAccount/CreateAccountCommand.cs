using MediatR;
using SharedKernel.Core.Enums;

namespace auth_service.Application.Features.Auth.Commands.CreateAccount
{
    public record CreateAccountCommand(
        string Email,
        string Password,
        UserRole UserRole
    // string FullName,
    // Gender Gender
    // DateOnly DateOfBirth,
    // string? StudentCode,
    // string? ClassCode,
    // string? FacultyCode
    ) : IRequest<Guid>;
}