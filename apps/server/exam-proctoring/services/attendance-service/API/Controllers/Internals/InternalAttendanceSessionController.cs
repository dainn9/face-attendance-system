using attendance_service.API.Contracts.AttendanceSessions;
using attendance_service.Application.Contracts.AttendanceSession;
using attendance_service.Application.Features.Attendances.Commands.CheckInAttendance;
using attendance_service.Application.Features.Attendances.Queries.GetAttendanceRecords;
using BuildingBlocks.Results;
using BuildingBlocks.Security.Internal;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace attendance_service.API.Controllers.Internals
{
    [ApiController]
    [Authorize(AuthenticationSchemes = InternalAuthenticationHandler.SchemeName)]
    [Route("api/v1/internal/attendance-sessions")]
    public class InternalAttendanceSessionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InternalAttendanceSessionController(IMediator mediator) => _mediator = mediator;

        // GET: api/v1/internal/attendance-sessions/{attendanceSessionId}/records
        [HttpGet("{attendanceSessionId:guid}/records")]
        public async Task<IActionResult> GetAttendanceRecords(
            Guid attendanceSessionId,
            CancellationToken cancellationToken)
        {
            var query = new GetAttendanceRecordsQuery(attendanceSessionId);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(new ApiResponse<Dictionary<Guid, AttendanceRecordDto>>
            {
                Success = true,
                Message = "Attendance records retrieved successfully",
                Data = result
            });
        }

        // POST: api/v1/internal/attendance-sessions/{attendanceSessionId}/check-in
        [HttpPost("{attendanceSessionId:guid}/check-in")]
        public async Task<IActionResult> CheckInAttendance(
            Guid attendanceSessionId,
            [FromBody] CheckInAttendanceRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CheckInAttendanceCommand(
                AttendanceSessionId: attendanceSessionId,
                StudentId: request.StudentId,
                Confidence: request.Confidence
            );

            await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Student checked in successfully",
            });
        }
    }
}