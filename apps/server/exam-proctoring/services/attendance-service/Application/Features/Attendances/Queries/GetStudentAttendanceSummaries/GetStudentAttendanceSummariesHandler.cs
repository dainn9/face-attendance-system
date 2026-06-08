using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Application.Contracts.AttendanceSession;
using MediatR;

namespace attendance_service.Application.Features.Attendances.Queries.GetStudentAttendanceSummaries
{
    public class GetStudentAttendanceSummariesHandler : IRequestHandler<GetStudentAttendanceSummariesQuery, Dictionary<Guid, StudentAttendanceSummaryDto>>
    {
        private readonly IAttendanceSessionReadRepository _attendanceSessionReadRepository;

        public GetStudentAttendanceSummariesHandler(IAttendanceSessionReadRepository attendanceSessionReadRepository)
        {
            _attendanceSessionReadRepository = attendanceSessionReadRepository;
        }

        public async Task<Dictionary<Guid, StudentAttendanceSummaryDto>> Handle(GetStudentAttendanceSummariesQuery request, CancellationToken cancellationToken) =>
        await _attendanceSessionReadRepository.GetStudentAttendanceSummariesAsync(
                request.CourseSectionId,
                request.StudentIds,
                cancellationToken
            );
    }
}