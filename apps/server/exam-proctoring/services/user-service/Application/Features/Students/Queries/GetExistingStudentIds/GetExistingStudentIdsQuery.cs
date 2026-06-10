using MediatR;

namespace user_service.Application.Features.Students.Queries.GetExistingStudentIds
{
    public record GetExistingStudentIdsQuery(
        IEnumerable<Guid> StudentIds
    ) : IRequest<IReadOnlyList<Guid>>;
}