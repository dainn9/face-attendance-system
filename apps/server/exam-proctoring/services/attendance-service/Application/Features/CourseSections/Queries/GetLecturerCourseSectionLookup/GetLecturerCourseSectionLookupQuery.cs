
using attendance_service.Application.Contracts;
using MediatR;

namespace attendance_service.Application.Features.CourseSections.Queries.GetLecturerCourseSectionLookup
{
    public record GetLecturerCourseSectionLookupQuery(Guid LecturerId) : IRequest<IReadOnlyList<LecturerCourseSectionLookupDto>>;
}