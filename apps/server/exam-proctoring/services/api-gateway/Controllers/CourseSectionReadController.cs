using api_gateway.Clients;
using api_gateway.Contracts.Attendance;
using api_gateway.Contracts.Users;
using BuildingBlocks.Extensions;
using BuildingBlocks.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_gateway.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/course-sections")]
    public class CourseSectionReadController : ControllerBase
    {

        private readonly AttendanceClient _attendanceClient;
        private readonly UserClient _userClient;

        public CourseSectionReadController(AttendanceClient attendanceClient, UserClient userClient)
        {
            _attendanceClient = attendanceClient;
            _userClient = userClient;
        }

        // GET: api/v1/course-sections/{courseSectionId}
        [HttpGet("{courseSectionId:guid}")]
        public async Task<IActionResult> GetCourseSectionDetailById(
                Guid courseSectionId,
                CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var role = User.GetUserRole();

            var courseSection = await _attendanceClient.GetCourseSectionDetailAsync(
                courseSectionId, userId, role, cancellationToken);
            if (courseSection == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Course section with ID {courseSectionId} not found"
                });
            }

            var lecturer = await _userClient.GetLecturerByIdAsync(courseSection.LecturerId, cancellationToken);

            var result = new CourseSectionDetailResponse(
                Id: courseSection.Id,
                SubjectName: courseSection.SubjectName,
                Credits: courseSection.Credits,
                CourseSectionCode: courseSection.CourseSectionCode,
                IsActive: courseSection.IsActive,
                Semester: courseSection.Semester,
                AcademicYear: courseSection.AcademicYear,
                MaxCapacity: courseSection.MaxCapacity,
                StudentCount: courseSection.StudentCount,
                Lecturer: lecturer ?? new LecturerDto(Guid.Empty, "Unknown", "Unknown"),
                Schedules: courseSection.Schedules
            );

            return Ok(new ApiResponse<CourseSectionDetailResponse>
            {
                Success = true,
                Message = "Course section retrieved successfully",
                Data = result
            });
        }

    }
}