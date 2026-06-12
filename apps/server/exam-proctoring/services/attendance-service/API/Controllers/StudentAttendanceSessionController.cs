using attendance_service.Application.Contracts.AttendanceSession;
using attendance_service.Application.Features.Attendances.Queries.GetAttendanceCheckInInfo;
using attendance_service.Application.Features.Attendances.Queries.GetStudentCourseSectionAttendanceRecords;
using BuildingBlocks.Extensions;
using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace attendance_service.API.Controllers
{
    [ApiController]
    [Route("api/v1/student")]
    public class StudentAttendanceSessionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StudentAttendanceSessionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/v1/student/attendance-session/{attendanceSessionId}/check-in-info
        [HttpGet("attendance-session/{attendanceSessionId:guid}/check-in-info")]
        public async Task<IActionResult> GetAttendanceCheckInInfo(
            Guid attendanceSessionId,
            CancellationToken cancellationToken)
        {
            var query = new GetAttendanceCheckInInfoQuery(attendanceSessionId);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(new ApiResponse<AttendanceCheckInInfoDto>
            {
                Success = true,
                Message = "Attendance check-in info retrieved successfully",
                Data = result
            });
        }

        // GET: api/v1/student/course-section/{courseSectionId}/attendance-records
        [HttpGet("course-section/{courseSectionId:guid}/attendance-records")]
        public async Task<IActionResult> GetAttendanceRecords(
            Guid courseSectionId,
            CancellationToken cancellationToken)
        {
            var studentId = User.GetUserId();
            var query = new GetStudentCourseSectionAttendanceRecordsQuery(
                studentId,
                courseSectionId
            );

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(new ApiResponse<IReadOnlyList<StudentAttendanceRecordDto>>
            {
                Success = true,
                Message = "Attendance records retrieved successfully",
                Data = result
            });
        }
    }
}