using api_gateway.Clients;
using api_gateway.Contracts;
using BuildingBlocks.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Core.Enums;

namespace api_gateway.Controllers
{
    [ApiController]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [Route("api/v1/admin/users")]
    public class AdminUsersController : ControllerBase
    {
        private readonly UserClient _userClient;
        private readonly AuthClient _authClient;

        public AdminUsersController(UserClient userClient, AuthClient authClient)
        {
            _userClient = userClient;
            _authClient = authClient;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
        {
            // Bước 1 — Tạo account
            var userId = await _authClient.CreateAccountAsync(
                new CreateAccountRequest(
                    request.Email,
                    request.Password,
                    request.UserRole
                ), ct);

            // Bước 2 — Tạo profile, nếu fail thì rollback
            try
            {
                await _userClient.CreateUserAsync(
                    new CreateUserRequest(
                        userId,
                        request.UserCode,
                        request.FullName,
                        request.Gender,
                        request.DateOfBirth,
                        request.Email,
                        request.UserRole,
                        request.FacultyId,
                        request.MajorId
                    ), ct);
            }
            catch
            {
                // Rollback — xóa account vừa tạo
                await _authClient.DeleteAccountAsync(userId, ct);
                throw;
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Registration successful"
            });
        }
    }
}