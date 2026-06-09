using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Application.Contracts.AttendanceSession;
using MediatR;

namespace attendance_service.Application.Features.Attendances.Queries.GetAttendanceRecords
{
    public class GetAttendanceRecordsHandler : IRequestHandler<GetAttendanceRecordsQuery, Dictionary<Guid, AttendanceRecordDto>>
    {
        private readonly IAttendanceSessionReadRepository _attendanceSessionReadRepository;

        public GetAttendanceRecordsHandler(IAttendanceSessionReadRepository attendanceSessionReadRepository)
        {
            _attendanceSessionReadRepository = attendanceSessionReadRepository;
        }

        public Task<Dictionary<Guid, AttendanceRecordDto>> Handle(GetAttendanceRecordsQuery request, CancellationToken cancellationToken)
        => _attendanceSessionReadRepository.GetAttendanceRecordsBySessionIdAsync(request.AttendanceSessionId, cancellationToken);
    }
}