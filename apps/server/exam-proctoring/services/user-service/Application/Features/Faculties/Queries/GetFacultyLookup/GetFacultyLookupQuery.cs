using MediatR;
using user_service.Application.Contracts;

namespace user_service.Application.Features.Faculties.Queries.GetFacultyLookup
{
    public record GetFacultyLookupQuery() : IRequest<IReadOnlyList<FacultyLookupDto>>;
}