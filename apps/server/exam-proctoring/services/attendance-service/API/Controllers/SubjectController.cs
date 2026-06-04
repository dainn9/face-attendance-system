using attendance_service.API.Contracts.Subjects;
using attendance_service.Application.Contracts;
using attendance_service.Application.Features.Subjects.Commands.CreateSubject;
using attendance_service.Application.Features.Subjects.Commands.UpdateSubject;
using attendance_service.Application.Features.Subjects.Queries.GetById;
using attendance_service.Application.Features.Subjects.Queries.GetSubjectLookup;
using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Core.Enums;

namespace attendance_service.API.Controllers
{
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ApiController]
    [Route("api/v1/subjects")]
    public class SubjectController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SubjectController(IMediator mediator) => _mediator = mediator;

        // POST: api/v1/subjects
        [HttpPost]
        public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectRequest request)
        {
            var command = new CreateSubjectCommand(
                request.FacultyId,
                request.Name,
                request.Code,
                request.Credits
            );

            var subjectId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { subjectId }, new ApiResponse<Guid>
            {
                Success = true,
                Message = "Subject created successfully",
                Data = subjectId
            });
        }

        // PUT: api/v1/subjects/{subjectId}
        [HttpPut("{subjectId:guid}")]
        public async Task<IActionResult> UpdateSubject(Guid subjectId, [FromBody] UpdateSubjectRequest request)
        {
            var command = new UpdateSubjectCommand(
                subjectId,
                request.FacultyId,
                request.Name,
                request.Code,
                request.Credits
            );

            await _mediator.Send(command);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Subject updated successfully"
            });
        }

        // GET: api/v1/subjects/{subjectId}
        [HttpGet("{subjectId:guid}")]
        public async Task<IActionResult> GetById(Guid subjectId)
        {
            var query = new GetByIdQuery(subjectId);
            var subjectDto = await _mediator.Send(query);

            return Ok(new ApiResponse<SubjectDto>
            {
                Success = true,
                Message = "Subject retrieved successfully",
                Data = subjectDto
            });
        }

        // ── Lookup ──────────────────────────────────────────

        [HttpGet("lookup")]
        public async Task<IActionResult> GetSubjectLookup([FromQuery] string? keyword)
        {
            var query = new GetSubjectLookupQuery(keyword);
            var subjectLookups = await _mediator.Send(query);

            return Ok(new ApiResponse<IReadOnlyList<SubjectLookupDto>>
            {
                Success = true,
                Message = "Subject lookups retrieved successfully",
                Data = subjectLookups
            });
        }

        // [HttpGet("paged")]
        // public async Task<IActionResult> GetPaged([FromQuery] int page = 1)
        // {
        //     var query = new GetPagedQuery(page);
        //     var pagedResult = await _mediator.Send(query);

        //     return Ok(new ApiResponse<PagedResult<SubjectDto>>
        //     {
        //         Success = true,
        //         Message = "Paged subjects retrieved successfully",
        //         Data = pagedResult
        //     });
        // }

    }
}