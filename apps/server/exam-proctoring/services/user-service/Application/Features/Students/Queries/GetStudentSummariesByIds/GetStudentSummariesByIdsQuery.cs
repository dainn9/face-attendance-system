using MediatR;
using user_service.Application.Contracts.Students;

namespace user_service.Application.Features.Students.Queries.GetStudentSummariesByIds
{
    public record GetStudentSummariesByIdsQuery(
        IEnumerable<Guid> StudentIds
    ) : IRequest<Dictionary<Guid, StudentSummaryDto>>;
}