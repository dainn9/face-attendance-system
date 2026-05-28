using auth_service.API.Contracts;
using auth_service.Application.Contracts;
using auth_service.Application.Features.Auth.Commands.ChangePassword;
using auth_service.Application.Features.Auth.Commands.Login;
using auth_service.Application.Features.Auth.Commands.Logout;
using auth_service.Application.Features.Auth.Commands.RefreshToken;
using auth_service.Application.Features.Auth.Commands.Register;
using auth_service.Application.Features.Auth.Queries.GetMe;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Extensions;
using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Core.Enums;

namespace auth_service.API.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator) => _mediator = mediator;

        // POST: api/v1/auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAdmin([FromBody] LoginRequest request)
        {
            var command = new LoginCommand(request.Email, request.Password);
            var result = await _mediator.Send(command);
            SetAuthCookies(result);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Login successful"
            });
        }

        // POST: api/v1/auth/logout
        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refresh"];
            if (string.IsNullOrEmpty(refreshToken))
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Logout successful"
                });

            await _mediator.Send(new LogoutCommand(refreshToken));

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            };

            Response.Cookies.Delete("access", cookieOptions);
            Response.Cookies.Delete("refresh", cookieOptions);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Logout successful"
            });
        }

        // POST: api/v1/auth/refresh
        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refresh"];
            if (string.IsNullOrEmpty(refreshToken))
                throw new UnauthorizedException("Missing refresh token", ErrorCodes.InvalidRefreshToken);

            var result = await _mediator.Send(new RefreshTokenCommand(refreshToken));
            SetAuthCookies(result);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Token refreshed"
            });
        }

        // =============================
        // Password
        // =============================
        // POST: api/v1/auth/change-password
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = User.GetUserId();
            var command = new ChangePasswordCommand(userId, request.CurrentPassword, request.NewPassword, request.ConfirmPassword);
            await _mediator.Send(command);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Password changed successfully"
            });
        }

        // =============================
        // Me
        // =============================
        // GET: api/v1/auth/me
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.GetUserId();
            var query = new GetMeQuery(userId);
            var result = await _mediator.Send(query);
            return Ok(new ApiResponse<MeResponse>
            {
                Success = true,
                Data = result
            });
        }

        // =============================
        // Admin-only
        // =============================
        // POST: api/v1/auth/register
        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var command = new RegisterCommand(
                request.Email,
                request.Password,
                request.UserRole,
                request.FullName,
                request.Gender,
                request.DateOfBirth,
                request.StudentCode,
                request.LecturerCode,
                request.FacultyCode,
                request.MajorCode
            );

            await _mediator.Send(command);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Registration successful"
            });
        }

        // =============================
        // Helpers
        // =============================
        private void SetAuthCookies(AuthResponse result)
        {
            Response.Cookies.Append("access", result.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                MaxAge = result.AccessTokenExpiresIn,
                Path = "/"
            });
            Response.Cookies.Append("refresh", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                MaxAge = result.RefreshTokenExpiresIn,
                Path = "/"
            });
        }
    }
}