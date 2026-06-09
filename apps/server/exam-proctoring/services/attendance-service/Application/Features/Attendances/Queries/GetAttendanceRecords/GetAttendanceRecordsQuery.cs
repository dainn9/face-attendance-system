using attendance_service.Application.Contracts.AttendanceSession;
using MediatR;

namespace attendance_service.Application.Features.Attendances.Queries.GetAttendanceRecords
{
    public record GetAttendanceRecordsQuery(
        Guid AttendanceSessionId
    ) : IRequest<Dictionary<Guid, AttendanceRecordDto>>;
}