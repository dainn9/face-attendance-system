using MediatR;
using SharedKernel.Core.Enums;

namespace auth_service.Application.Features.Auth.Commands.Register
{
    public record RegisterCommand(
        string Email,
        string Password,
        UserRole UserRole,
        string FullName,
        Gender Gender,
        DateOnly DateOfBirth,
        string? StudentCode,
        string? LecturerCode,
        string? FacultyCode,
        string? MajorCode
    ) : IRequest;
}