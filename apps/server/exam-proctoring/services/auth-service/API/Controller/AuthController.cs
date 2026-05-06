using auth_service.API.Contracts;
using auth_service.API.Extensions;
using auth_service.Application.Contracts;
using auth_service.Application.Features.Auth.Commands.ChangePassword;
using auth_service.Application.Features.Auth.Commands.LoginAdmin;
using auth_service.Application.Features.Auth.Commands.LoginProctor;
using auth_service.Application.Features.Auth.Commands.LoginProfile;
using auth_service.Application.Features.Auth.Commands.Logout;
using auth_service.Application.Features.Auth.Commands.RefreshToken;
using auth_service.Application.Features.Auth.Queries.GetMe;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Extensions;
using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace auth_service.API.Controller
{
    [ApiController]
    [Route("api/v1/auths")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator) => _mediator = mediator;

        // POST: api/v1/auths/login-admin
        [HttpPost("login-admin")]
        public async Task<IActionResult> LoginAdmin([FromBody] LoginRequest request)
        {
            var command = new LoginAdminCommand(request.Email, request.Password);
            var result = await _mediator.Send(command);
            SetAuthCookies(result);
            return Ok(new { message = "Login successful" });
        }

        // POST: api/v1/auths/login-profile
        [HttpPost("login-profile")]
        public async Task<IActionResult> LoginProfile([FromBody] LoginRequest request)
        {
            var command = new LoginProfileCommand(request.Email, request.Password);
            var result = await _mediator.Send(command);
            SetAuthCookies(result);
            return Ok(new { message = "Login successful" });
        }

        // POST: api/v1/auths/login-proctor
        [HttpPost("login-proctor")]
        public async Task<IActionResult> LoginProctor([FromBody] LoginRequest request)
        {
            var command = new LoginProctorCommand(request.Email, request.Password);
            var result = await _mediator.Send(command);
            SetAuthCookies(result);
            return Ok(new { message = "Login successful" });
        }

        // POST: api/v1/auths/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var sessionType = User.GetSessionType();
            var userId = User.GetUserId();

            await _mediator.Send(new LogoutCommand(userId, sessionType));

            Response.Cookies.Delete("access");
            Response.Cookies.Delete("refresh");
            return Ok(new { message = "Logout successful" });
        }

        // POST: api/v1/auths/refresh
        [Authorize]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refresh"];
            if (string.IsNullOrEmpty(refreshToken))
                throw new UnauthorizedException("Missing refresh token", ErrorCodes.InvalidRefreshToken);

            var sessionType = User.GetSessionType();
            var userId = User.GetUserId();

            var result = await _mediator.Send(new RefreshTokenCommand(userId, refreshToken, sessionType));
            SetAuthCookies(result);
            return Ok(new { message = "Token refreshed" });
        }

        // =============================
        // Password
        // =============================
        // POST: api/v1/auths/change-password
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = User.GetUserId();
            var command = new ChangePasswordCommand(userId, request.CurrentPassword, request.NewPassword, request.ConfirmPassword);
            await _mediator.Send(command);
            return Ok(new { message = "Password changed successfully" });
        }

        // =============================
        // Me
        // =============================
        // GET: api/v1/auths/me
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
                TraceId = HttpContext.TraceIdentifier,
                Data = result
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