using MediatR;
using user_service.Application.Contracts.Users;

namespace user_service.Application.Features.Users.Queries.GetLecturersByIds
{
    public record GetLecturersByIdsQuery(IEnumerable<Guid> UserIds) : IRequest<Dictionary<Guid, UserLookupDto>>;
}