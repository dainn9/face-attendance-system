using MediatR;
using user_service.Application.Contracts;

namespace user_service.Application.Features.Users.Queries.GetLecturerLookupByFacultyId
{
    public record GetLecturerLookupByFacultyIdQuery(Guid? FacultyId, string? Keyword) : IRequest<IReadOnlyList<UserLookupDto>>;
}