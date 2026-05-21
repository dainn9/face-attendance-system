using MediatR;
using user_service.Domain.Enum;

namespace user_service.Application.Features.Users.Commands.CreateUser
{
    public record CreateUserCommand(Guid UserId, string FullName, Gender Gender, DateOnly DateOfBirth, string Email) : IRequest;
}