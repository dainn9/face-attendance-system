using api_gateway.Clients;
using api_gateway.Contracts.Attendance;
using api_gateway.Contracts.Enrollments;
using api_gateway.Contracts.Users;
using BuildingBlocks.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Core.Enums;

namespace api_gateway.Controllers.Admin
{
    [ApiController]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [Route("api/v1/admin/course-sections")]
    public class AdminCourseSectionController : ControllerBase
    {
        private readonly AttendanceClient _attendanceClient;
        private readonly UserClient _userClient;


        public AdminCourseSectionController(AttendanceClient attendanceClient, UserClient userClient)
        {
            _attendanceClient = attendanceClient;
            _userClient = userClient;
        }

        // GET: api/v1/admin/course-sections
        [HttpGet]
        public async Task<IActionResult> GetCourseSections([FromQuery] GetCourseSectionPagedRequest request, CancellationToken cancellationToken)
        {
            var pagedCourseSections = await _attendanceClient.GetCourseSectionPagedAsync(request, cancellationToken);

            // Lấy tất cả lecturerId từ course sections
            var lecturerIds = pagedCourseSections.Items
                .Select(cs => cs.LecturerId)
                .Distinct()
                .ToList();

            // Lấy thông tin giảng viên
            var lecturers = lecturerIds.Any()
                ? await _userClient.GetLecturersByIdsAsync(lecturerIds, cancellationToken)
                : new Dictionary<Guid, UserLookupDto>();

            // Map lecturer info vào course sections
            var items = pagedCourseSections.Items
            .Select(cs => cs with
            {
                LecturerName = lecturers.TryGetValue(cs.LecturerId, out var lecturer) ? lecturer.FullName : "Unknown"
            })
            .ToList();

            var result = new PagedResult<CourseSectionPagedDto>
            {
                Items = items,
                TotalCount = pagedCourseSections.TotalCount,
                Page = pagedCourseSections.Page,
                PageSize = pagedCourseSections.PageSize
            };

            return Ok(new ApiResponse<PagedResult<CourseSectionPagedDto>>
            {
                Success = true,
                Message = "Course sections retrieved successfully",
                Data = result
            });
        }

        // POST: api/v1/admin/course-sections
        [HttpPost]
        public async Task<IActionResult> CreateCourseSection([FromBody] CreateCourseSectionRequest request, CancellationToken cancellationToken)
        {
            if (!await _userClient.CheckLecturerExistsAsync(request.LecturerId, cancellationToken))
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Lecturer with ID {request.LecturerId} does not exist"
                });
            }

            var courseSectionId = await _attendanceClient.CreateCourseSectionAsync(request, cancellationToken);

            return Ok(new ApiResponse<Guid>
            {
                Success = true,
                Message = "Course section created successfully",
                Data = courseSectionId
            });
        }

        // GET: api/v1/admin/course-sections/{courseSectionId}/students
        [HttpGet("{courseSectionId:guid}/students")]
        public async Task<IActionResult> GetEnrolledStudentsByCourseSectionId(
            Guid courseSectionId,
            [FromQuery] GetEnrolledStudentIdsPageRequest request,
            CancellationToken cancellationToken)
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

            var students = studentIdsPaged.Items.Any()
                ? await _userClient.GetStudentSummariesByIdsAsync(studentIdsPaged.Items, cancellationToken)
                : new Dictionary<Guid, StudentSummaryDto>();

            var result = new PagedResult<StudentSummaryDto>
            {
                Items = studentIdsPaged.Items
                    .Where(id => students.ContainsKey(id))
                    .Select(id => students[id])
                    .ToList(),
                TotalCount = studentIdsPaged.TotalCount,
                Page = studentIdsPaged.Page,
                PageSize = studentIdsPaged.PageSize
            };

            return Ok(new ApiResponse<PagedResult<StudentSummaryDto>>
            {
                Success = true,
                Message = "Enrolled students retrieved successfully",
                Data = result
            });
        }

        // POST: api/v1/admin/course-sections/{courseSectionId}/enrollments
        [HttpPost("{courseSectionId:guid}/enrollments")]
        public async Task<IActionResult> EnrollStudentsToCourseSection(
            Guid courseSectionId,
            [FromBody] EnrollStudentsRequest request,
            CancellationToken cancellationToken
        )
        {
            var studentIds = request.StudentIds.Distinct().ToList();
            if (!studentIds.Any())
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "No valid student IDs provided"
                });
            }

            var existingStudentIds = await _userClient.GetExistingStudentIdsAsync(studentIds, cancellationToken);
            var missingStudentIds = studentIds.Except(existingStudentIds).ToList();

            if (missingStudentIds.Any())
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "One or more students do not exist"
                });
            }

            await _attendanceClient.EnrollStudentsAsync(courseSectionId, existingStudentIds, cancellationToken);

            return NoContent();
        }
    }
}