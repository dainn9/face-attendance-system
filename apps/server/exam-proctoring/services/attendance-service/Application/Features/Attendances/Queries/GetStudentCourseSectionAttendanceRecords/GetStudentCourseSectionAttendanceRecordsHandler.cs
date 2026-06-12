using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Application.Contracts.AttendanceSession;
using MediatR;

namespace attendance_service.Application.Features.Attendances.Queries.GetStudentCourseSectionAttendanceRecords
{
    public class GetStudentCourseSectionAttendanceRecordsHandler : IRequestHandler<GetStudentCourseSectionAttendanceRecordsQuery, IReadOnlyList<StudentAttendanceRecordDto>>
    {
        private readonly IAttendanceSessionReadRepository _attendanceSessionReadRepository;

        public GetStudentCourseSectionAttendanceRecordsHandler(IAttendanceSessionReadRepository attendanceSessionReadRepository)
        {
            _attendanceSessionReadRepository = attendanceSessionReadRepository;
        }

        public Task<IReadOnlyList<StudentAttendanceRecordDto>> Handle(GetStudentCourseSectionAttendanceRecordsQuery request, CancellationToken cancellationToken)
        => _attendanceSessionReadRepository.GetStudentAttendanceRecordsByCourseSectionIdAsync(
            request.StudentId,
            request.CourseSectionId,
            cancellationToken
        );
    }
}