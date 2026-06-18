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
    [Route("api/v1/student/attendance-session")]
    public class StudentAttendanceSessionController : ControllerBase
    {
        private readonly AttendanceClient _attendanceClient;
        private readonly FaceClient _faceClient;

        public StudentAttendanceSessionController(AttendanceClient attendanceClient, FaceClient faceClient)
        {
            _attendanceClient = attendanceClient;
            _faceClient = faceClient;
        }

        // POST: api/v1/student/attendance-session/{attendanceSessionId}/check-in
        [HttpPost("{attendanceSessionId:guid}/check-in")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CheckInAttendance(
            Guid attendanceSessionId,
            [FromForm] VerifyFaceRequest request,
            CancellationToken cancellationToken)
        {
            var studentId = User.GetUserId();

            if (FileValidator.IsInvalidVideo(request.Video))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Video is required.",
                    ErrorCode = "INVALID_FACE_IMAGES"
                });
            }

            if (!await _faceClient.GetStatusAsync(studentId, cancellationToken))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Please register your face before checking in.",
                    ErrorCode = "FACE_NOT_REGISTERED"
                });
            }

            var result = await _faceClient.VerifyFaceAsync(
                studentId,
                request,
                cancellationToken
            );

            if (!result.Match)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Face verification failed. Please try again.",
                    ErrorCode = "FACE_VERIFICATION_FAILED"
                });
            }

            var checkInRequest = new CheckInAttendanceRequest(studentId, result.Score);

            await _attendanceClient.CheckInAttendanceAsync(
                attendanceSessionId,
                checkInRequest,
                cancellationToken
            );

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Checked in successfully",
            });
        }
    }
}