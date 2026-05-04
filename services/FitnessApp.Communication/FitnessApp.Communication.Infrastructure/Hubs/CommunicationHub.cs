using FitnessApp.Communication.Application.Features.Users.Commands.UpdateUserStatus;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FitnessApp.Communication.Infrastructure.Hubs
{
    public class CommunicationHub : Hub
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CommunicationHub> _logger;

        public CommunicationHub(
            IMediator mediator,
            ILogger<CommunicationHub> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userIdStr = Context.UserIdentifier;
            _logger.LogInformation("SignalR User Connected: {UserId}", userIdStr ?? "Anonymous");

            if (Guid.TryParse(userIdStr, out var userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
                await _mediator.Send(new UpdateUserStatusCommand(userId, true));
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userIdStr = Context.UserIdentifier;
            if (Guid.TryParse(userIdStr, out var userId))
            {
                await _mediator.Send(new UpdateUserStatusCommand(userId, false));
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinRoom(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"room_{roomId}");
        }

        public async Task LeaveRoom(string roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"room_{roomId}");
        }
    }
}
