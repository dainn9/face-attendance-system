using MediatR;
using user_service.Application.Contracts;

namespace user_service.Application.Features.Majors.Queries.GetMajorLookupByFacultyId
{
    public record GetMajorLookupByFacultyIdQuery(Guid FacultyId) : IRequest<IReadOnlyList<MajorLookupDto>>;
}