using auth_service.Domain.Enum;
using MediatR;
using SharedKernel.Core.Enums;

namespace auth_service.Application.Features.Auth.Commands.Register
{
    public record RegisterCommand(string Email, string Password, UserRole UserRole, string FullName, Gender Gender, DateOnly DateOfBirth) : IRequest;

}