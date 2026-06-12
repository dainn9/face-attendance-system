using api_gateway.Clients;
using api_gateway.Contracts.Attendance;
using api_gateway.Contracts.Users;
using BuildingBlocks.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Core.Enums;

namespace api_gateway.Controllers.Lecturer
{
    [ApiController]
    [Authorize(Roles = nameof(UserRole.Lecturer))]
    [Route("api/v1/lecturer/course-sections/{courseSectionId:guid}/attendance-sessions")]
    public class LecturerAttendanceSessionController : ControllerBase
    {
        private readonly AttendanceClient _attendanceClient;
        private readonly UserClient _userClient;

        public LecturerAttendanceSessionController(AttendanceClient attendanceClient, UserClient userClient)
        {
            _attendanceClient = attendanceClient;
            _userClient = userClient;
        }

        // GET: api/v1/lecturer/course-sections/{courseSectionId}/attendance-sessions/{attendanceSessionId}/students?page=1&pageSize=10
        [HttpGet("{attendanceSessionId:guid}/students")]
        public async Task<IActionResult> GetAttendanceSessionStudents(
            Guid courseSectionId,
            Guid attendanceSessionId,
            [FromQuery] CourseSectionStudentsPagedRequest request,
            CancellationToken cancellationToken
        )
        {
            var studentIdsPaged = await _attendanceClient.GetEnrolledStudentIdsPagedAsync(
                courseSectionId,
                request.Page,
                request.PageSize,
                cancellationToken
            );

            if (studentIdsPaged == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Course section with ID {courseSectionId} not found"
                });
            }

            var studentBasics = studentIdsPaged.Items.Any()
                ? await _userClient.GetStudentBasicsByIdsAsync(studentIdsPaged.Items, cancellationToken)
                : new Dictionary<Guid, StudentBasicDto>();

            var attendanceRecords = await _attendanceClient.GetAttendanceRecordsAsync(attendanceSessionId, cancellationToken);

            var items = studentIdsPaged.Items
                .Where(id => studentBasics.ContainsKey(id))
                .Select(id =>
                {
                    var student = studentBasics[id];

                    attendanceRecords.TryGetValue(id, out var attendanceRecord);

                    return new AttendanceSessionStudentDto(
                        UserId: id,
                        StudentCode: student.UserCode,
                        FullName: student.FullName,
                        Email: student.Email,
                        AttendanceStatus: attendanceRecord != null ? attendanceRecord.Status : null,
                        Confidence: attendanceRecord?.Confidence ?? null,
                        CheckedInAt: attendanceRecord?.CheckedInAt ?? null
                    );
                })
                .ToList();

            var pagedResult = new PagedResult<AttendanceSessionStudentDto>
            {
                Items = items,
                TotalCount = studentIdsPaged.TotalCount,
                Page = studentIdsPaged.Page,
                PageSize = studentIdsPaged.PageSize
            };

            return Ok(new ApiResponse<PagedResult<AttendanceSessionStudentDto>>
            {
                Success = true,
                Message = "Attendance session students retrieved successfully",
                Data = pagedResult
            });
        }
    }
}