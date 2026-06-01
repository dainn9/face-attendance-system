using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Core.Enums;
using user_service.API.Contracts;
using user_service.Application.Contracts;
using user_service.Application.Features.Faculties.Commands.CreateFaculty;
using user_service.Application.Features.Faculties.Commands.UpdateFaculty;
using user_service.Application.Features.Faculties.Queries.GetFaculties;
using user_service.Application.Features.Faculties.Queries.GetFacultyDetail;
using user_service.Application.Features.Majors.Commands;
using user_service.Application.Features.Majors.Commands.UpdateMajor;

namespace user_service.API.Controllers
{
    [ApiController]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [Route("api/v1/faculties")]
    public class FacultyController : ControllerBase
    {
        private readonly IMediator _mediator;
        public FacultyController(IMediator mediator) => _mediator = mediator;

        // ── Faculties ──────────────────────────────────────────

        // POST: api/v1/faculties
        [HttpPost]
        public async Task<IActionResult> CreateFaculty([FromBody] CreateFacultyRequest request)
        {
            var command = new CreateFacultyCommand(
                request.Name,
                request.Code
            );

            var facultyId = await _mediator.Send(command);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Faculty created successfully",
                Data = facultyId
            });
        }

        // GET: api/v1/faculties
        [HttpGet]
        public async Task<IActionResult> GetFaculties()
        {
            var query = new GetFacultiesQuery();
            var faculties = await _mediator.Send(query);

            return Ok(new ApiResponse<IReadOnlyList<FacultyDto>>
            {
                Success = true,
                Message = "Faculties retrieved successfully",
                Data = faculties
            });
        }

        // GET: api/v1/faculties/{facultyId}
        [HttpGet("{facultyId:guid}")]
        public async Task<IActionResult> GetById(Guid facultyId)
        {
            var query = new GetFacultyDetailQuery(facultyId);
            var facultyDto = await _mediator.Send(query);

            return Ok(new ApiResponse<FacultyDetailDto>
            {
                Success = true,
                Message = "Faculty retrieved successfully",
                Data = facultyDto
            });
        }

        // PUT: api/v1/faculties/{facultyId}
        [HttpPut("{facultyId:guid}")]
        public async Task<IActionResult> UpdateFaculty(Guid facultyId, [FromBody] UpdateFacultyRequest request)
        {
            var command = new UpdateFacultyCommand(
                facultyId,
                request.Name,
                request.Code
            );

            await _mediator.Send(command);
            return NoContent();
        }

        // ── Majors ──────────────────────────────────────────

        // POST: api/v1/faculties/{facultyId}/majors
        [HttpPost("{facultyId:guid}/majors")]
        public async Task<IActionResult> AddMajor(Guid facultyId, [FromBody] AddMajorRequest request)
        {
            var command = new CreateMajorCommand(
                facultyId,
                request.Name,
                request.Code
            );

            await _mediator.Send(command);

            return NoContent();
        }

        // PUT: api/v1/faculties/{facultyId}/majors/{majorId}
        [HttpPut("{facultyId:guid}/majors/{majorId:guid}")]
        public async Task<IActionResult> UpdateMajor(Guid facultyId, Guid majorId, [FromBody] UpdateMajorRequest request)
        {
            var command = new UpdateMajorCommand(
                facultyId,
                majorId,
                request.Name,
                request.Code
            );

            await _mediator.Send(command);
            return NoContent();
        }

        // [HttpGet("{facultyId:guid}")]
        // public async Task<IActionResult> GetById(Guid facultyId)
        // {
        //     var query = new GetFacultyByIdQuery(facultyId);
        //     var facultyDto = await _mediator.Send(query);

        //     return Ok(new ApiResponse<FacultyDto>
        //     {
        //         Success = true,
        //         Message = "Course retrieved successfully",
        //         Data = facultyDto
        //     });
        // }
    }
}