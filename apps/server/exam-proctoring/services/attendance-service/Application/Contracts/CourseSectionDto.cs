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

    public record ScheduleConflictDto(
        string CourseSectionCode,
        DayOfWeek DayOfWeek,
        TimeOnly StartTime,
        TimeOnly EndTime,
        string Room,
        ScheduleConflictReason ConflictReason
    );

    public enum ScheduleConflictReason
    {
        Room,
        Lecturer
    }

    public record ScheduleDetailDto(
        Guid Id,
        DayOfWeek DayOfWeek,
        TimeOnly StartTime,
        TimeOnly EndTime,
        string Room
    );

    public record CourseSectionDetailDto(
        Guid Id,
        string SubjectName,
        int Credits,
        string CourseSectionCode,
        Guid LecturerId,
        bool IsActive,
        Semester Semester,
        string AcademicYear,
        int MaxCapacity,
        int StudentCount,
        List<ScheduleDetailDto> Schedules
    );
}