using MediatR;

namespace auth_service.Application.Features.Auth.Queries.GetStatusByIds
{
    public record GetStatusByIdsQuery(IEnumerable<Guid> UserIds) : IRequest<Dictionary<Guid, bool>>;
}