using auth_service.Application.Contracts;
using MediatR;

namespace auth_service.Application.Features.Auth.Queries.GetMe
{
    public record GetMeQuery(Guid UserId) : IRequest<MeResponse>;
}