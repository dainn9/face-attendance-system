using MediatR;
using user_service.Application.Contracts;

namespace user_service.Application.Features.Users.Queries.GetStudentSummariesByIds
{
    public record GetStudentSummariesByIdsQuery(
        IEnumerable<Guid> StudentIds
    ) : IRequest<Dictionary<Guid, StudentSummaryDto>>;
}