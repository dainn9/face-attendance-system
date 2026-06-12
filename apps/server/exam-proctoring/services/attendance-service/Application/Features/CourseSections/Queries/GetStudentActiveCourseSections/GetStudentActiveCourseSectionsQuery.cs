using attendance_service.Application.Contracts;
using MediatR;

namespace attendance_service.Application.Features.CourseSections.Queries.GetStudentActiveCourseSections
{
    public record GetStudentActiveCourseSectionsQuery(
        Guid StudentId
    ) : IRequest<IReadOnlyList<StudentCourseSectionDto>>;
}