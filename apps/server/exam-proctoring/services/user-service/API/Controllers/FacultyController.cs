using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Core.Enums;
using user_service.API.Contracts;
using user_service.Application.Contracts;
using user_service.Application.Features.Faculties.Commands.CreateFaculty;
using user_service.Application.Features.Faculties.Queries.GetFaculties;

namespace user_service.API.Controllers
{
    [ApiController]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [Route("api/v1/faculties")]
    public class FacultyController : ControllerBase
    {
        private readonly IMediator _mediator;
        public FacultyController(IMediator mediator) => _mediator = mediator;

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