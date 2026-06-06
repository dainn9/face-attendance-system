using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using user_service.API.Contracts.Lecturers;
using user_service.Application.Contracts.Students;
using user_service.Application.Contracts.Users;
using user_service.Application.Features.Students.Queries.GetStudentLookup;
using user_service.Application.Features.Users.Queries.GetLecturerLookupByFacultyId;

namespace user_service.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/users")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UserController(IMediator mediator) => _mediator = mediator;

        // [HttpGet("profile")]
        // public async Task<IActionResult> GetUserProfile()
        // {
        //     var userId = User.GetUserId();

        //     var query = new GetUserProfileQuery(userId);
        //     var result = await _mediator.Send(query);
        //     return Ok(new ApiResponse<UserDto>
        //     {
        //         Success = true,
        //         Message = "User profile retrieved successfully",
        //         Data = result
        //     });
        // }

        [HttpGet("lookup/lecturers")]
        public async Task<IActionResult> GetLecturerLookupByFacultyId([FromQuery] GetLecturerLookupRequest request, CancellationToken cancellationToken)
        {
            var query = new GetLecturerLookupByFacultyIdQuery(request.FacultyId, request.Keyword);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(new ApiResponse<IReadOnlyList<UserLookupDto>>
            {
                Success = true,
                Message = "Lecturer lookup retrieved successfully",
                Data = result
            });
        }

        [HttpGet("lookup/students")]
        public async Task<IActionResult> GetStudentLookup([FromQuery] string? keyWord, CancellationToken cancellationToken)
        {
            var query = new GetStudentLookupQuery(keyWord);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(new ApiResponse<IReadOnlyList<StudentLookupDto>>
            {
                Success = true,
                Message = "Student lookup retrieved successfully",
                Data = result
            });
        }
    }
}