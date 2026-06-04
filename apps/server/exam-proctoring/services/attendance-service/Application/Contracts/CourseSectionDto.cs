using attendance_service.Domain.Enums;

namespace attendance_service.Application.Contracts
{
    public record CourseSectionPagedDto(
        Guid Id,
        string SubjectName,
        string SubjectCode,
        string CourseSectionCode,
        Guid LecturerId,
        Guid? FacultyId,
        bool IsActive,
        Semester Semester,
        string AcademicYear,
        int StudentCount,
        ScheduleDto? FirstSchedule
    );

    public sealed record ScheduleDto(
        DayOfWeek DayOfWeek,
        TimeOnly StartTime,
        TimeOnly EndTime,
        string Room
    );
}