using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Application.Contracts.AttendanceSession;
using BuildingBlocks.Exceptions;
using MediatR;

namespace attendance_service.Application.Features.Attendances.Queries.GetAttendanceCheckInInfo
{
    public class GetAttendanceCheckInInfoHandler : IRequestHandler<GetAttendanceCheckInInfoQuery, AttendanceCheckInInfoDto>
    {
        private readonly IAttendanceSessionReadRepository _attendanceSessionReadRepository;

        public GetAttendanceCheckInInfoHandler(IAttendanceSessionReadRepository attendanceSessionReadRepository) =>
            _attendanceSessionReadRepository = attendanceSessionReadRepository;

        public async Task<AttendanceCheckInInfoDto> Handle(GetAttendanceCheckInInfoQuery request, CancellationToken cancellationToken)
        => await _attendanceSessionReadRepository.GetAttendanceCheckInInfoAsync(request.AttendanceSessionId, cancellationToken)
            ?? throw new EntityNotFoundException("Attendance session", request.AttendanceSessionId);
    }
}