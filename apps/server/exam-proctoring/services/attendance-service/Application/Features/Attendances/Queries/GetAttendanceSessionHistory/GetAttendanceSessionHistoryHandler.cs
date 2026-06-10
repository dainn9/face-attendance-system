using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Application.Contracts.AttendanceSession;
using BuildingBlocks.Results;
using MediatR;

namespace attendance_service.Application.Features.Attendances.Queries.GetAttendanceSessionHistory
{
    public class GetAttendanceSessionHistoryHandler : IRequestHandler<GetAttendanceSessionHistoryQuery, PagedResult<AttendanceSessionHistoryDto>>
    {
        private readonly IAttendanceSessionReadRepository _attendanceSessionReadRepository;

        public GetAttendanceSessionHistoryHandler(IAttendanceSessionReadRepository attendanceSessionReadRepository) =>
            _attendanceSessionReadRepository = attendanceSessionReadRepository;

        public Task<PagedResult<AttendanceSessionHistoryDto>> Handle(GetAttendanceSessionHistoryQuery request, CancellationToken cancellationToken)
        => _attendanceSessionReadRepository.GetAttendanceSessionHistoryAsync(
            request.CourseSectionId,
            request.Page,
            request.PageSize,
            cancellationToken
        );
    }
}