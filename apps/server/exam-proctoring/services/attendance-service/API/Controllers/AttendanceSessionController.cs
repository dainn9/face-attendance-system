using attendance_service.API.Contracts.AttendanceSessions;
using attendance_service.Application.Contracts.AttendanceSession;
using attendance_service.Application.Features.Attendances.Commands.CloseAttendanceSession;
using attendance_service.Application.Features.Attendances.Commands.CreateAttendanceSession;
using attendance_service.Application.Features.Attendances.Queries.GetAttendanceSessionById;
using BuildingBlocks.Extensions;
using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Core.Enums;

namespace attendance_service.API.Controllers
{
    [Authorize(Roles = nameof(UserRole.Lecturer))]
    [ApiController]
    [Route("api/v1/attendance-sessions")]
    public class AttendanceSessionController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AttendanceSessionController(IMediator mediator) => _mediator = mediator;

        // POST: api/v1/attendance-sessions
        [HttpPost]
        public async Task<IActionResult> CreateAttendanceSession(
            [FromBody] CreateAttendanceSessionRequest request,
            CancellationToken cancellationToken)
        {
            var lecturerId = User.GetUserId();

            var command = new CreateAttendanceSessionCommand(
                lecturerId,
                request.CourseSectionId);

            var sessionId = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponse<Guid>
            {
                Success = true,
                Message = "Attendance session created successfully",
                Data = sessionId
            });
        }

        // POST: api/v1/attendance-sessions/{attendanceSessionId}/close
        [HttpPost("{attendanceSessionId:guid}/close")]
        public async Task<IActionResult> CloseAttendanceSession(
            Guid attendanceSessionId,
            CancellationToken cancellationToken)
        {
            var lecturerId = User.GetUserId();

            var command = new CloseAttendanceSessionCommand(
                lecturerId,
                attendanceSessionId);

            await _mediator.Send(command, cancellationToken);

            return NoContent();
        }

        // GET: api/v1/attendance-sessions/{attendanceSessionId}
        [HttpGet("{attendanceSessionId:guid}")]
        public async Task<IActionResult> GetAttendanceSessionById(
            Guid attendanceSessionId,
            CancellationToken cancellationToken)
        {
            var query = new GetAttendanceSessionByIdQuery(attendanceSessionId);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(new ApiResponse<AttendanceSessionDetailDto>
            {
                Success = true,
                Message = "Attendance session retrieved successfully",
                Data = result
            });
        }
    }
}