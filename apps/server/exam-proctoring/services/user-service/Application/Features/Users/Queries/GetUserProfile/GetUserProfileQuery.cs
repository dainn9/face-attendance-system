using MediatR;
using user_service.Application.Contracts.Users;

namespace user_service.Application.Features.Users.Queries.GetUserProfile
{
    public record GetUserProfileQuery(Guid UserId) : IRequest<UserDto>;
}