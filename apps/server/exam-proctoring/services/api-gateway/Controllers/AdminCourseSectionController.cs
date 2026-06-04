using api_gateway.Clients;
using api_gateway.Contracts.Attendance;
using api_gateway.Contracts.Users;
using BuildingBlocks.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Core.Enums;

namespace api_gateway.Controllers
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
    }
}