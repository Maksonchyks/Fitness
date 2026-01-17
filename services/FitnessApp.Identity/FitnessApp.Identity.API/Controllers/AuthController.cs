using FitnessApp.Identity.API.Common.Constants;
using FitnessApp.Identity.API.Common.Models;
using FitnessApp.Identity.Application.DTOs;
using FitnessApp.Identity.Application.UseCases.Auth.Login;
using FitnessApp.Identity.Application.UseCases.Auth.RefreshToken;
using FitnessApp.Identity.Application.UseCases.Auth.Register;
using FitnessApp.Identity.Application.DTOs.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegisterRequest = FitnessApp.Identity.Application.DTOs.Requests.RegisterRequest;
using LoginRequest = FitnessApp.Identity.Application.DTOs.Requests.LoginRequest;
using ForgotPasswordRequest = FitnessApp.Identity.Application.DTOs.Requests.ForgotPasswordRequest;
using ResetPasswordRequest = FitnessApp.Identity.Application.DTOs.Requests.ResetPasswordRequest;

namespace FitnessApp.Identity.API.Controllers
{
    [ApiController]
    [Route(ApiRoutes.Auth.Base)]
    [ApiVersion(ApiVersions.V1)]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IMediator mediator, ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

 
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            _logger.LogInformation("Registration request for email: {Email}", request.Email);

            var command = new RegisterCommand
            {
                Email = request.Email,
                Username = request.Username,
                Password = request.Password,
                ConfirmPassword = request.ConfirmPassword,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                FitnessGoal = request.FitnessGoal,
                IpAddress = GetClientIpAddress()
            };

            var result = await _mediator.Send(command);

            _logger.LogInformation("User registered successfully: {UserId}", result.UserId);

            return Ok(result);
        }

        
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("Login attempt for email: {Email}", request.Email);

            var command = new LoginCommand
            {
                Email = request.Email,
                Password = request.Password,
                IpAddress = GetClientIpAddress()
            };

            var result = await _mediator.Send(command);

            _logger.LogInformation("User logged in: {UserId}", result.UserId);

            SetRefreshTokenCookie(result.RefreshToken);

            return Ok(new
            {
                result.UserId,
                result.Email,
                result.Username,
                result.FullName,
                result.AccessToken,
                result.ExpiresAt,
                result.Roles
            });
        }


        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            _logger.LogInformation("Refresh token request");

            var command = new RefreshTokenCommand
            {
                RefreshToken = request.RefreshToken ?? GetRefreshTokenFromCookie(),
                IpAddress = GetClientIpAddress()
            };

            if (string.IsNullOrEmpty(command.RefreshToken))
            {
                return Unauthorized(new ErrorResponse("INVALID_TOKEN", "Refresh token is required"));
            }

            var result = await _mediator.Send(command);

            SetRefreshTokenCookie(result.RefreshToken);

            _logger.LogInformation("Tokens refreshed successfully");

            return Ok(result);
        }

        
        [HttpPost("revoke-token")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            // TODO: оновленнятокена зробити
            await Task.CompletedTask;

            _logger.LogInformation("Token revoked by user: {UserId}", userId);

            return Ok(new { message = "Token revoked successfully" });
        }

       
        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            // TODO: забув пароля
            await Task.CompletedTask;

            _logger.LogInformation("Password reset requested for email: {Email}", request.Email);

            return Ok(new { message = "If the email exists, a reset link will be sent" });
        }

        
        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            // TODO:  ресет пасворда
            await Task.CompletedTask;

            return Ok(new { message = "Password has been reset successfully" });
        }

        private string? GetClientIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return Request.Headers["X-Forwarded-For"];
            }

            return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        private string? GetRefreshTokenFromCookie()
        {
            return Request.Cookies["refreshToken"];
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
                SameSite = SameSiteMode.Strict,
                Secure = !Request.Host.Host.Contains("localhost"),
                Path = "/api/auth/refresh-token"
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
