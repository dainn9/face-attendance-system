using api_gateway.Clients;
using api_gateway.Contracts.Attendance;
using api_gateway.Contracts.Users;
using BuildingBlocks.Extensions;
using BuildingBlocks.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Core.Enums;

namespace api_gateway.Controllers.Student
{
    [ApiController]
    [Authorize(Roles = nameof(UserRole.Student))]
    [Route("api/v1/student")]
    public class StudentCourseSectionController : ControllerBase
    {
        private readonly AttendanceClient _attendanceClient;
        private readonly UserClient _userClient;

        public StudentCourseSectionController(AttendanceClient attendanceClient, UserClient userClient)
        {
            _attendanceClient = attendanceClient;
            _userClient = userClient;
        }

        // GET: api/v1/student/my-course-sections
        [HttpGet("my-course-sections")]
        public async Task<IActionResult> GetMyCourseSectionsAsync(CancellationToken cancellationToken)
        {
            var studentId = User.GetUserId();
            var courseSections = await _attendanceClient.GetStudentActiveCourseSectionsAsync(studentId, cancellationToken);

            // Lấy tất cả lecturerId từ course sections
            var lecturerIds = courseSections
                .Select(x => x.LecturerId)
                .Distinct()
                .ToList();

            var lecturers = lecturerIds.Count > 0
                ? await _userClient.GetLecturersByIdsAsync(lecturerIds, cancellationToken)
                : new Dictionary<Guid, UserLookupDto>();

            // Map lecturer info vào course sections
            courseSections = courseSections.Select(cs => cs with
            {
                LecturerName = lecturers.TryGetValue(cs.LecturerId, out var lecturer) ? lecturer.FullName : "Unknown"
            })
            .ToList();

            return Ok(new ApiResponse<IReadOnlyCollection<StudentCourseSectionDto>>
            {
                Success = true,
                Message = "My course sections retrieved successfully.",
                Data = courseSections
            });
        }

    }
}