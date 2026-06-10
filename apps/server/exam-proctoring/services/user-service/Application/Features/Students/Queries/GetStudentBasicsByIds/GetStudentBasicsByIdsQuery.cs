using MediatR;
using user_service.Application.Contracts.Students;

namespace user_service.Application.Features.Students.Queries.GetStudentBasicsByIds
{
    public record GetStudentBasicsByIdsQuery(
        IEnumerable<Guid> StudentIds
    ) : IRequest<Dictionary<Guid, StudentBasicDto>>;
}
