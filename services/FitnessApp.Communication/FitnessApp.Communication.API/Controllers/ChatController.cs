using FitnessApp.Communication.Application.Features.Chats.Commands.CreateChatRoom;
using FitnessApp.Communication.Application.Features.Chats.Commands.SendMessage;
using FitnessApp.Communication.Application.Features.Chats.Commands.MarkAsRead;
using FitnessApp.Communication.Application.Features.Chats.Commands.MarkAsRead.DTOs;
using FitnessApp.Communication.Application.Features.Chats.Queries.GetMessages;
using FitnessApp.Communication.Application.Features.Chats.Queries.GetRooms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FitnessApp.Communication.API.Controllers
{
    [Authorize]
    public class ChatController : BaseApiController
    {
        [HttpGet("rooms/{userId}")]
        public async Task<IActionResult> GetRooms(Guid userId)
        {
            return Ok(await Mediator.Send(new GetRoomsQuery(userId)));
        }

        [HttpGet("rooms/{roomId}/messages")]
        public async Task<IActionResult> GetMessages(Guid roomId)
        {
            return Ok(await Mediator.Send(new GetMessagesQuery(roomId)));
        }

        [HttpPost("rooms")]
        public async Task<IActionResult> CreateRoom([FromBody] CreateChatRoomCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("messages")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpPost("rooms/{roomId}/read")]
        public async Task<IActionResult> MarkAsRead(Guid roomId, [FromBody] MarkAsReadRequest request)
        {
            return Ok(await Mediator.Send(new MarkAsReadCommand(roomId, request.UserId)));
        }
    }
}
