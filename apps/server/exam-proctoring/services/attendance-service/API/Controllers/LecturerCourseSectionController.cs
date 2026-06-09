using attendance_service.API.Contracts.AttendanceSessions;
using attendance_service.API.Contracts.CourseSections;
using attendance_service.Application.Contracts;
using attendance_service.Application.Contracts.AttendanceSession;
using attendance_service.Application.Features.Attendances.Queries.GetAttendanceSessionHistory;
using attendance_service.Application.Features.CourseSections.Queries.GetCoureseSectionPagedByLecturerId;
using attendance_service.Application.Features.CourseSections.Queries.GetLecturerCourseSectionLookup;
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
    [Route("api/v1/lecturer/course-sections")]
    public class LecturerCourseSectionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LecturerCourseSectionController(IMediator mediator) => _mediator = mediator;

        // GET: api/v1/lecturer/course-sections/my
        [HttpGet("my")]
        public async Task<IActionResult> GetMyCourseSections([FromQuery] GetLecturerCourseSectionRequest request, CancellationToken cancellationToken)
        {
            var lecturerId = User.GetUserId();
            var query = new GetLecturerCourseSectionsQuery(
                lecturerId,
                request.Page,
                request.PageSize,
                request.SearchQuery,
                request.Semester,
                request.AcademicYear,
                request.IsActive
            );

            var result = await _mediator.Send(query, cancellationToken);

            return Ok(new ApiResponse<PagedResult<LecturerCourseSectionDto>>
            {
                Success = true,
                Message = "Course sections retrieved successfully",
                Data = result
            });
        }

        // GET: api/v1/lecturer/course-sections/lookup
        [HttpGet("lookup")]
        public async Task<IActionResult> GetLookup(CancellationToken cancellationToken)
        {
            var lecturerId = User.GetUserId();
            var result = await _mediator.Send(
                new GetLecturerCourseSectionLookupQuery(lecturerId),
                cancellationToken);

            return Ok(new ApiResponse<IReadOnlyList<LecturerCourseSectionLookupDto>>
            {
                Success = true,
                Message = "Course section lookup retrieved successfully",
                Data = result
            });
        }

        // GET: api/v1/lecturer/course-sections/{courseSectionId}/attendance-sessions
        [HttpGet("{courseSectionId:guid}/attendance-sessions")]
        public async Task<IActionResult> GetAttendanceSessionHistory(
            Guid courseSectionId,
            [FromQuery] GetAttendanceSessionHistoryRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetAttendanceSessionHistoryQuery(
                    courseSectionId,
                    request.Page,
                    request.PageSize
                ),
                cancellationToken);

            return Ok(new ApiResponse<PagedResult<AttendanceSessionHistoryDto>>
            {
                Success = true,
                Message = "Attendance session history retrieved successfully",
                Data = result
            });
        }
    }
}