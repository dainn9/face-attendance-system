namespace api_gateway.Contracts.Attendance
{
    public record CreateCourseSectionRequest(
        Guid SubjectId,
        string CourseSectionCode,
        int Semester,
        string AcademicYear,
        Guid LecturerId,
        int MaxCapacity,
        IReadOnlyList<CreateScheduleDto> Schedules
    );

    public record CreateScheduleDto(
        DayOfWeek DayOfWeek,
        TimeOnly StartTime,
        TimeOnly EndTime,
        string Room
    );
}