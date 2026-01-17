using FitnessApp.Identity.API.Common.Constants;
using FitnessApp.Identity.API.Common.Models;
using FitnessApp.Identity.Application.DTOs.Requests;
using FitnessApp.Identity.Application.DTOs;
using FitnessApp.Identity.Application.UseCases.Users.GetUser;
using FitnessApp.Identity.Application.UseCases.Users.UpdateProfile;
using FitnessApp.Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FitnessApp.Identity.Application.DTOs.Responses;
using UserResponse = FitnessApp.Identity.Application.DTOs.UserResponse;

namespace FitnessApp.Identity.API.Controllers
{
    [ApiController]
    [Route(ApiRoutes.Account.Base)]
    [ApiVersion(ApiVersions.V1)]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IMediator mediator, ILogger<AccountController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

       
        [HttpGet("profile")]
        [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserProfileResponse>> GetProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            _logger.LogInformation("Get profile request for user: {UserId}", userId);

            var query = new GetUserQuery { UserId = userId.Value };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        
        [HttpPut("profile")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserResponse>> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            _logger.LogInformation("Update profile request for user: {UserId}", userId);

            var command = new UpdateProfileCommand
            {
                UserId = userId.Value,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                FitnessGoal = request.FitnessGoal,
                ProfileImageUrl = request.ProfileImageUrl
            };

            var result = await _mediator.Send(command);

            _logger.LogInformation("Profile updated for user: {UserId}", userId);

            return Ok(result);
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}
