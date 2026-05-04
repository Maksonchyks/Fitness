using FitnessApp.Communication.Application.Features.Notifications.Commands.MarkAsRead;
using FitnessApp.Communication.Application.Features.Notifications.Queries.GetHistory;
using FitnessApp.Communication.Application.Features.Notifications.Queries.GetPreferences;
using FitnessApp.Communication.Application.Features.Notifications.Commands.UpdatePreferences;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

using FitnessApp.Communication.Application.Interfaces;

namespace FitnessApp.Communication.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class NotificationController : BaseApiController
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [HttpGet("preferences/{userId}")]
        public async Task<IActionResult> GetPreferences(Guid userId)
        {
            return Ok(await Mediator.Send(new GetPreferencesQuery(userId)));
        }

        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetHistory(Guid userId)
        {
            return Ok(await Mediator.Send(new GetHistoryQuery(userId)));
        }

        [HttpPost("preferences")]
        public async Task<IActionResult> UpdatePreferences([FromBody] UpdatePreferencesCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("history/{id}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var result = await Mediator.Send(new MarkAsReadCommand(id));
            if (!result) return NotFound();
            return Ok();
        }
        [HttpPost("test")]
        public async Task<IActionResult> SendTestNotification()
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdStr, out var userId))
            {
                await _notificationService.SendLiveReminderAsync(
                    userId, 
                    "Тестове сповіщення", 
                    "Вітаємо! Ваша система сповіщень працює правильно.", 
                    "Nutrition");
                return Ok(new { Message = "Notification sent" });
            }
            return BadRequest("Could not identify user");
        }
    }
}
