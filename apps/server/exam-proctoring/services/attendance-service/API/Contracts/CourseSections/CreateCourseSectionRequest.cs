using attendance_service.Application.Contracts;
using attendance_service.Domain.Enums;

namespace attendance_service.API.Contracts.CourseSections
{
    public record CreateCourseSectionRequest(
        Guid SubjectId,
        string CourseSectionCode,
        Semester Semester,
        string AcademicYear,
        Guid LecturerId,
        int MaxCapacity,
        IReadOnlyList<ScheduleDto> Schedules
    );
}