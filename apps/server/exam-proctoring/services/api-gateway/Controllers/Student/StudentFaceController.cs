using api_gateway.Clients;
using api_gateway.Contracts.Attendance;
using api_gateway.Contracts.Face;
using api_gateway.Helpers;
using BuildingBlocks.Extensions;
using BuildingBlocks.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Core.Enums;

namespace api_gateway.Controllers.Student
{
    [ApiController]
    [Authorize(Roles = nameof(UserRole.Student))]
    [Route("api/v1/student/faces")]
    public class StudentFaceController : ControllerBase
    {
        private readonly FaceClient _faceClient;

        public StudentFaceController(FaceClient faceClient)
        {
            _faceClient = faceClient;
        }

        // POST: api/v1/student/faces/register
        [HttpPost("register")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> RegisterFaceAsync(
            [FromForm] RegisterFaceRequest request,
            CancellationToken cancellationToken)
        {
            if (FileValidator.IsInvalidImage(request.Left) ||
                FileValidator.IsInvalidImage(request.Center) ||
                FileValidator.IsInvalidImage(request.Right))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Left, center and right images are required.",
                    ErrorCode = "INVALID_FACE_IMAGES"
                });
            }

            var studentId = User.GetUserId();
            await _faceClient.RegisterFaceAsync(studentId, request, cancellationToken);
            return NoContent();
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatusAsync()
        {
            var studentId = User.GetUserId();
            var isRegistered = await _faceClient.GetStatusAsync(studentId);
            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Face registration status retrieved successfully.",
                Data = isRegistered
            });
        }

        [HttpGet("challenge")]
        public async Task<IActionResult> GetChallengeAsync()
        {
            var challenge = await _faceClient.GetChallengeAsync();
            return Ok(new ApiResponse<ChallengeDto>
            {
                Success = true,
                Message = "Face challenge retrieved successfully.",
                Data = challenge
            });
        }
    }
}