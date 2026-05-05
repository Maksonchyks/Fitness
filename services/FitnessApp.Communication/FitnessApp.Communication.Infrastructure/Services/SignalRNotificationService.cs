using FitnessApp.Communication.Application.Interfaces;
using FitnessApp.Communication.Domain.Entities;
using FitnessApp.Communication.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace FitnessApp.Communication.Infrastructure.Services
{
    public class SignalRNotificationService : INotificationService
    {
        private readonly IHubContext<CommunicationHub> _hubContext;

        public SignalRNotificationService(IHubContext<CommunicationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyNewMessageAsync(ChatMessage message)
        {
            await _hubContext.Clients.Group($"room_{message.ChatRoomId}").SendAsync("ReceiveMessage", message);
        }

        public async Task NotifyMessagesReadAsync(Guid roomId, Guid userId)
        {
            await _hubContext.Clients.Group($"room_{roomId}").SendAsync("MessagesRead", new { RoomId = roomId, ReaderId = userId });
        }

        public async Task NotifyUserStatusChangedAsync(Guid userId, bool isActive)
        {
            await _hubContext.Clients.All.SendAsync("UserStatusChanged", new { UserId = userId, IsActive = isActive });
        }

        public async Task SendLiveReminderAsync(Guid userId, string title, string message, string type)
        {
            await _hubContext.Clients.Group($"user_{userId}").SendAsync("ReceiveNotification", new
            {
                Title = title,
                Message = message,
                Timestamp = DateTime.UtcNow,
                Type = type
            });
        }
    }
}
