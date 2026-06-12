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
    [Route("api/v1/lecturer/course-sections")]
    public class LecturerCoureSectionController : ControllerBase
    {
        private readonly AttendanceClient _attendanceClient;
        private readonly UserClient _userClient;

        public LecturerCoureSectionController(AttendanceClient attendanceClient, UserClient userClient)
        {
            _attendanceClient = attendanceClient;
            _userClient = userClient;
        }

        // GET: api/v1/lecturer/course-sections/{courseSectionId}/students
        [HttpGet("{courseSectionId:guid}/students")]
        public async Task<IActionResult> GetCourseSectionStudents(
            Guid courseSectionId,
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

            var attendanceSummaries = studentIdsPaged.Items.Any()
                ? await _attendanceClient.GetStudentAttendanceSummariesAsync(
                    courseSectionId,
                    studentIdsPaged.Items,
                    cancellationToken
                )
                : new Dictionary<Guid, StudentAttendanceSummaryDto>();

            var items = studentIdsPaged.Items
                .Where(id => studentBasics.ContainsKey(id))
                .Select(id =>
                {
                    var student = studentBasics[id];

                    attendanceSummaries.TryGetValue(id, out var summary);

                    return new LecturerCourseSectionStudentDto(
                        student.UserId,
                        student.UserCode,
                        student.FullName,
                        student.Email,
                        summary?.PresentSessions ?? 0,
                        summary?.TotalSessions ?? 0,
                        summary?.AttendanceRate ?? 0
                    );
                })
                .ToList();

            var pagedResult = new PagedResult<LecturerCourseSectionStudentDto>
            {
                Items = items,
                TotalCount = studentIdsPaged.TotalCount,
                Page = studentIdsPaged.Page,
                PageSize = studentIdsPaged.PageSize
            };

            return Ok(new ApiResponse<PagedResult<LecturerCourseSectionStudentDto>>
            {
                Success = true,
                Message = "Course section students retrieved successfully",
                Data = pagedResult
            });
        }
    }
}