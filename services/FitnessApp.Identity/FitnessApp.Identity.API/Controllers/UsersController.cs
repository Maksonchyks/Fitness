using FitnessApp.Identity.API.Common.Constants;
using FitnessApp.Identity.API.Common.Models;
using FitnessApp.Identity.Application.DTOs;
using FitnessApp.Identity.Application.DTOs.Requests;
using FitnessApp.Identity.Application.UseCases.Users.GetUser;
using FitnessApp.Identity.Application.UseCases.Users.UpdateProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Identity.API.Controllers
{
    [ApiController]
    [Route(ApiRoutes.Users.Base)]
    [ApiVersion(ApiVersions.V1)]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IMediator mediator, ILogger<UsersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        
        [HttpGet("{id:guid}", Name = "GetUser")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UserResponse>> GetUser(Guid id)
        {
            _logger.LogInformation("Get user request: {UserId}", id);

            var query = new GetUserQuery { UserId = id };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

       
        [HttpPut("{id:guid}/profile")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UserResponse>> UpdateProfile(
            Guid id,
            [FromBody] UpdateProfileRequest request)
        {
            _logger.LogInformation("Update profile request for user: {UserId}", id);

            var command = new UpdateProfileCommand
            {
                UserId = id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                FitnessGoal = request.FitnessGoal,
                ProfileImageUrl = request.ProfileImageUrl
            };

            var result = await _mediator.Send(command);

            _logger.LogInformation("Profile updated for user: {UserId}", id);

            return Ok(result);
        }

      
        [HttpGet]
        [Authorize(Policy = "RequireAdminRole")]
        [ProducesResponseType(typeof(List<UserResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserResponse>>> GetUsers()
        {
            var query = new FitnessApp.Identity.Application.UseCases.Users.GetUsers.GetUsersQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("{id:guid}/change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ChangePassword(
            Guid id,
            [FromBody] ChangePasswordRequest request)
        {
            var command = new FitnessApp.Identity.Application.UseCases.Users.ChangePassword.ChangePasswordCommand(
                id, request.CurrentPassword, request.NewPassword);
            await _mediator.Send(command);

            _logger.LogInformation("Password changed for user: {UserId}", id);

            return Ok(new { message = "Password changed successfully" });
        }

        [HttpPost("{id:guid}/deactivate")]
        [Authorize(Policy = "RequireAdminRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            var command = new FitnessApp.Identity.Application.UseCases.Users.DeactivateUser.DeactivateUserCommand(id);
            await _mediator.Send(command);

            _logger.LogInformation("Account deactivated: {UserId}", id);

            return Ok(new { message = "Account deactivated successfully" });
        }

        [HttpPost("{id:guid}/activate")]
        [Authorize(Policy = "RequireAdminRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Activate(Guid id)
        {
            var command = new FitnessApp.Identity.Application.UseCases.Users.ActivateUser.ActivateUserCommand(id);
            await _mediator.Send(command);

            _logger.LogInformation("Account activated: {UserId}", id);

            return Ok(new { message = "Account activated successfully" });
        }

        [HttpPost("{id:guid}/roles")]
        [Authorize(Policy = "RequireAdminRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ChangeRole(Guid id, [FromBody] ChangeRoleRequest request)
        {
            var command = new FitnessApp.Identity.Application.UseCases.Users.ChangeUserRole.ChangeUserRoleCommand(id, request.RoleName);
            await _mediator.Send(command);

            _logger.LogInformation("Account role changed: {UserId} to {Role}", id, request.RoleName);

            return Ok(new { message = "Account role changed successfully" });
        }
    }
}
