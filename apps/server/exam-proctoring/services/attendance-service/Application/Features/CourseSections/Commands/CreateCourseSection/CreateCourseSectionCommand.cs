using attendance_service.Application.Contracts;
using attendance_service.Domain.Enums;
using MediatR;

namespace attendance_service.Application.Features.CourseSections.Commands.CreateCourseSection
{
    public record CreateCourseSectionCommand(
        Guid SubjectId,
        string CourseSectionCode,
        Semester Semester,
        string AcademicYear,
        Guid LecturerId,
        int MaxCapacity,
        IReadOnlyList<ScheduleDto> Schedules
    ) : IRequest<Guid>;
}