using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Application.Contracts.AttendanceSession;
using BuildingBlocks.Exceptions;
using MediatR;

namespace attendance_service.Application.Features.Attendances.Queries.GetAttendanceSessionById
{
    public class GetAttendanceSessionByIdHandler : IRequestHandler<GetAttendanceSessionByIdQuery, AttendanceSessionDetailDto>
    {
        private readonly IAttendanceSessionReadRepository _attendanceSessionReadRepository;

        public GetAttendanceSessionByIdHandler(IAttendanceSessionReadRepository attendanceSessionReadRepository)
        {
            _attendanceSessionReadRepository = attendanceSessionReadRepository;
        }

        public async Task<AttendanceSessionDetailDto> Handle(GetAttendanceSessionByIdQuery request, CancellationToken cancellationToken)
        => await _attendanceSessionReadRepository.GetAttendanceSessionByIdAsync(request.Id, cancellationToken)
            ?? throw new EntityNotFoundException("Attendance session", request.Id);
    }
}