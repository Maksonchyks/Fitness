using FitnessApp.Communication.Domain.Entities;
using System.Threading.Tasks;

namespace FitnessApp.Communication.Application.Interfaces
{
    public interface INotificationService
    {
        Task NotifyNewMessageAsync(ChatMessage message);
        Task NotifyMessagesReadAsync(Guid roomId, Guid userId);
        Task NotifyUserStatusChangedAsync(Guid userId, bool isActive);
        Task SendLiveReminderAsync(Guid userId, string title, string message, string type);
    }
}
