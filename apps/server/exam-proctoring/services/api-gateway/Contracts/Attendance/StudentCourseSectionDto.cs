namespace api_gateway.Contracts.Attendance
{
    public record StudentCourseSectionDto(
        Guid Id,
        string SubjectName,
        string SubjectCode,
        string CourseSectionCode,
        Guid LecturerId,
        int Semester,
        string AcademicYear,
        List<ScheduleDto> Schedules,
        string LecturerName // merge từ User service
    );
}