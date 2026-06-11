using api_gateway.Clients;
using api_gateway.Contracts.Face;
using BuildingBlocks.Extensions;
using BuildingBlocks.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Core.Enums;

namespace api_gateway.Controllers
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
            if (IsInvalid(request.Left) ||
                IsInvalid(request.Center) ||
                IsInvalid(request.Right))
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

        private static bool IsInvalid(IFormFile? file)
        {
            return file is null ||
                   file.Length == 0 ||
                   !file.ContentType.StartsWith("image/");
        }
    }
}