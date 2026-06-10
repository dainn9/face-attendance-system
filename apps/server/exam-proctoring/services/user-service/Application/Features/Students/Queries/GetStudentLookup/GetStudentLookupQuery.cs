using MediatR;
using user_service.Application.Contracts.Students;

namespace user_service.Application.Features.Students.Queries.GetStudentLookup
{
    public record GetStudentLookupQuery(
        string? Keyword
    ) : IRequest<IReadOnlyList<StudentLookupDto>>;
}