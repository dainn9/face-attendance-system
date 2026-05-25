using attendance_service.API.Contracts;
using attendance_service.Application.Contracts;
using attendance_service.Application.Features.Courses.Commands.CreateCourse;
using attendance_service.Application.Features.Courses.Commands.UpdateCourse;
using attendance_service.Application.Features.Courses.Queries.GetById;
using attendance_service.Application.Features.Courses.Queries.GetPaged;
using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Core.Enums;

namespace attendance_service.API.Controllers
{
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ApiController]
    [Route("api/v1/courses")]
    public class CourseController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CourseController(IMediator mediator) => _mediator = mediator;

        // POST: api/v1/courses
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseRequest request)
        {
            var command = new CreateCourseCommand(
                request.Name,
                request.Code,
                request.Credits
            );

            var courseId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { courseId }, new ApiResponse<Guid>
            {
                Success = true,
                Message = "Course created successfully",
                Data = courseId
            });
        }

        // PUT: api/v1/courses/{courseId}
        [HttpPut("{courseId:guid}")]
        public async Task<IActionResult> UpdateCourse(Guid courseId, [FromBody] UpdateCourseRequest request)
        {
            var command = new UpdateCourseCommand(
                courseId,
                request.Name,
                request.Code,
                request.Credits
            );

            await _mediator.Send(command);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Course updated successfully"
            });
        }

        // GET: api/v1/courses/{courseId}
        [HttpGet("{courseId:guid}")]
        public async Task<IActionResult> GetById(Guid courseId)
        {
            var query = new GetByIdQuery(courseId);
            var courseDto = await _mediator.Send(query);

            return Ok(new ApiResponse<CourseDto>
            {
                Success = true,
                Message = "Course retrieved successfully",
                Data = courseDto
            });
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1)
        {
            var query = new GetPagedQuery(page);
            var pagedResult = await _mediator.Send(query);

            return Ok(new ApiResponse<PagedResult<CourseDto>>
            {
                Success = true,
                Message = "Paged courses retrieved successfully",
                Data = pagedResult
            });
        }

    }
}