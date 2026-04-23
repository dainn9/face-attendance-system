using MediatR;

namespace auth_service.Application.Features.Auth.Commands.ChangePassword
{
    public record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword, string ConfirmPassword) : IRequest<Task>;
}