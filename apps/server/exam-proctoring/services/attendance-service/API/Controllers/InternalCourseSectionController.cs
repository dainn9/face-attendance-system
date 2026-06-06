using attendance_service.API.Contracts.CourseSections;
using attendance_service.API.Contracts.Enrollments;
using attendance_service.Application.Contracts;
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

namespace attendance_service.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = InternalAuthenticationHandler.SchemeName)]
    [Route("api/v1/internal/course-sections")]
    public class InternalCourseSectionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InternalCourseSectionController(IMediator mediator) => _mediator = mediator;

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

        [HttpGet("{courseSectionId:guid}")]
        public async Task<IActionResult> GetCourseSectionDetail(Guid courseSectionId,
            CancellationToken cancellationToken)
        {
            var query = new GetCourseSectionDetailQuery(courseSectionId);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(new ApiResponse<CourseSectionDetailDto>
            {
                Success = true,
                Message = "Course section detail retrieved successfully",
                Data = result
            });
        }

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
    }
}