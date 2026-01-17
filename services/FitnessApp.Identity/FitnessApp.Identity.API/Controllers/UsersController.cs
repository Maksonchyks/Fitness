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

      
        [HttpPost("{id:guid}/change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ChangePassword(
            Guid id,
            [FromBody] ChangePasswordRequest request)
        {
            // TODO: зміна пароля
            await Task.CompletedTask;

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
            // TODO: деактивація профіля
            await Task.CompletedTask;

            _logger.LogInformation("Account deactivated: {UserId}", id);

            return Ok(new { message = "Account deactivated successfully" });
        }
    }
}
