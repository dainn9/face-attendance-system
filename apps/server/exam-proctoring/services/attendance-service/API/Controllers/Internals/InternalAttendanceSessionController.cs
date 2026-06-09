using attendance_service.Application.Contracts.AttendanceSession;
using attendance_service.Application.Features.Attendances.Queries.GetAttendanceRecords;
using attendance_service.Application.Features.Attendances.Queries.GetAttendanceSessionById;
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
    }
}