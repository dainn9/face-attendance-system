using api_gateway.Contracts.Users;

namespace api_gateway.Contracts.Attendance
{
    public record CourseSectionPagedDto(
        Guid Id,
        string SubjectName,
        string SubjectCode,
        string CourseSectionCode,
        Guid LecturerId,
        Guid? FacultyId,
        bool IsActive,
        int Semester,
        string AcademicYear,
        int StudentCount,
        ScheduleDto? FirstSchedule,
        string LecturerName // merge từ User service
    );

    public sealed record ScheduleDto(
       DayOfWeek DayOfWeek,
       TimeOnly StartTime,
       TimeOnly EndTime,
       string Room
    );

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
        int Semester,
        string AcademicYear,
        int MaxCapacity,
        int StudentCount,
        List<ScheduleDetailDto> Schedules
    );

    public record CourseSectionDetailResponse(
        Guid Id,
        string SubjectName,
        int Credits,
        string CourseSectionCode,
        bool IsActive,
        int Semester,
        string AcademicYear,
        int MaxCapacity,
        int StudentCount,
        LecturerDto Lecturer,
        List<ScheduleDetailDto> Schedules
    );
}