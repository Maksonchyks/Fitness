using System;
using FitnessApp.Communication.Domain.Common;

namespace FitnessApp.Communication.Domain.Entities
{
    public class UserNotification : Entity
    {
        public Guid UserId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Message { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }
        public bool IsRead { get; private set; }
        public string Type { get; private set; } = "Reminder"; // Reminder, Chat, etc.

        private UserNotification() { } // For EF

        public UserNotification(Guid userId, string title, string message, string type = "Reminder")
        {
            Guard.AgainstDefault(userId, nameof(userId));
            Guard.AgainstNullOrEmpty(title, nameof(title));
            Guard.AgainstNullOrEmpty(message, nameof(message));

            UserId = userId;
            Title = title;
            Message = message;
            Type = type;
            CreatedAt = DateTime.UtcNow;
            IsRead = false;
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
}
