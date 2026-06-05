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
}