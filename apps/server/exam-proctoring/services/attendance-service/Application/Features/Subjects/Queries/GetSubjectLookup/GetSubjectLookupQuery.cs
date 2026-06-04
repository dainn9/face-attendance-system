using attendance_service.Application.Contracts;
using MediatR;

namespace attendance_service.Application.Features.Subjects.Queries.GetSubjectLookup
{
    public record GetSubjectLookupQuery(
        string? Keyword
    ) : IRequest<IReadOnlyList<SubjectLookupDto>>;
}