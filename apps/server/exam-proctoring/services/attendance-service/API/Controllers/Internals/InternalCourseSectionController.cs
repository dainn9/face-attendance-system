using attendance_service.API.Contracts.CourseSections;
using attendance_service.API.Contracts.Enrollments;
using attendance_service.Application.Contracts;
using attendance_service.Application.Contracts.AttendanceSession;
using attendance_service.Application.Features.Attendances.Queries.GetStudentAttendanceSummaries;
using attendance_service.Application.Features.CourseSections.Commands.CreateCourseSection;
using attendance_service.Application.Features.CourseSections.Queries.GetCourseSectionDetail;
using attendance_service.Application.Features.CourseSections.Queries.GetCourseSectionPaged;
using attendance_service.Application.Features.Enrollments.Commands.AddEnrollments;
using attendance_service.Application.Features.Enrollments.Queries.GetEnrolledStudentIdsPaged;
using BuildingBlocks.Results;
using BuildingBlocks.Security.Internal;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Core.Enums;

namespace attendance_service.API.Controllers.Internals
{
    [ApiController]
    [Authorize(AuthenticationSchemes = InternalAuthenticationHandler.SchemeName)]
    [Route("api/v1/internal/course-sections")]
    public class InternalCourseSectionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InternalCourseSectionController(IMediator mediator) => _mediator = mediator;

        // GET: api/v1/internal/course-sections?searchQuery=math&page=1&pageSize=10&facultyId=123&semester=Fall&academicYear=2023&isActive=true
        [HttpGet]
        public async Task<IActionResult> GetCourseSections(
            [FromQuery] GetCourseSectionPagedRequest request,

            CancellationToken cancellationToken)
        {
            var query = new GetCourseSectionPagedQuery(
                request.Page,
                request.PageSize,
                request.SearchQuery,
                request.FacultyId,
                request.Semester,
                request.AcademicYear,
                request.IsActive
            );

            var result = await _mediator.Send(query, cancellationToken);

            return Ok(new ApiResponse<PagedResult<CourseSectionPagedDto>>
            {
                Success = true,
                Message = "Course sections retrieved successfully",
                Data = result
            });
        }

        // POST: api/v1/internal/course-sections
        [HttpPost]
        public async Task<IActionResult> CreateCourseSection([FromBody] CreateCourseSectionRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateCourseSectionCommand(
                request.SubjectId,
                request.CourseSectionCode,
                request.Semester,
                request.AcademicYear,
                request.LecturerId,
                request.MaxCapacity,
                request.Schedules
            );

            var courseSectionId = await _mediator.Send(command, cancellationToken);
            return Ok(new ApiResponse<Guid>
            {
                Success = true,
                Message = "Course section created successfully",
                Data = courseSectionId
            });
        }

        // GET: api/v1/internal/course-sections/{courseSectionId}?userId=123&role=Lecturer
        [HttpGet("{courseSectionId:guid}")]
        public async Task<IActionResult> GetCourseSectionDetail(
            Guid courseSectionId,
            [FromQuery] Guid userId,
            [FromQuery] UserRole role,
            CancellationToken cancellationToken)
        {
            var query = new GetCourseSectionDetailQuery(userId, role, courseSectionId);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(new ApiResponse<CourseSectionDetailDto>
            {
                Success = true,
                Message = "Course section detail retrieved successfully",
                Data = result
            });
        }

        // GET: api/v1/internal/course-sections/{courseSectionId}/students?page=1&pageSize=10
        [HttpGet("{courseSectionId:guid}/students")]
        public async Task<IActionResult> GetEnrolledStudentIdsPaged(
            Guid courseSectionId,
            [FromQuery] GetEnrolledStudentIdsPagedRequest request,
            CancellationToken cancellationToken
)
        {
            var query = new GetEnrolledStudentIdsPagedQuery(courseSectionId, request.Page, request.PageSize);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(new ApiResponse<PagedResult<Guid>>
            {
                Success = true,
                Message = "Enrolled student IDs retrieved successfully",
                Data = result
            });
        }

        // POST: api/v1/internal/course-sections/{courseSectionId}/enrollments
        [HttpPost("{courseSectionId:guid}/enrollments")]
        public async Task<IActionResult> AddEnrollments(Guid courseSectionId, [FromBody] IReadOnlyList<Guid> studentIds, CancellationToken cancellationToken)
        {
            var command = new AddEnrollmentsCommand(courseSectionId, studentIds);
            await _mediator.Send(command, cancellationToken);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Enrollments added successfully",
            });
        }

        // POST: api/v1/internal/course-sections/{courseSectionId}/students/attendance-summaries
        [HttpPost("{courseSectionId:guid}/students/attendance-summaries")]
        public async Task<IActionResult> GetStudentAttendanceSummariesByIds(
            Guid courseSectionId,
            [FromBody] IReadOnlyList<Guid> studentIds,
            CancellationToken cancellationToken)
        {
            var query = new GetStudentAttendanceSummariesQuery(courseSectionId, studentIds);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(new ApiResponse<Dictionary<Guid, StudentAttendanceSummaryDto>>
            {
                Success = true,
                Message = "Student attendance summaries retrieved successfully",
                Data = result
            });
        }
    }
}