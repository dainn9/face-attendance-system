using attendance_service.Application.Contracts.AttendanceSession;
using MediatR;

namespace attendance_service.Application.Features.Attendances.Queries.GetStudentCourseSectionAttendanceRecords
{
    public record GetStudentCourseSectionAttendanceRecordsQuery(
        Guid StudentId,
        Guid CourseSectionId
    ) : IRequest<IReadOnlyList<StudentAttendanceRecordDto>>;
}