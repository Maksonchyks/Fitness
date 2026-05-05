using System;
using FitnessApp.Communication.Domain.Common;

namespace FitnessApp.Communication.Domain.Entities
{
    public class ChatMessage : Entity
    {
        public Guid ChatRoomId { get; private set; }
        public Guid SenderId { get; private set; }
        public string SenderName { get; private set; } = string.Empty;
        public string Text { get; private set; } = string.Empty;
        public DateTime Timestamp { get; private set; }
        public bool IsRead { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public ChatRoom? ChatRoom { get; private set; }

        private ChatMessage() { } // For EF

        public ChatMessage(Guid chatRoomId, Guid senderId, string senderName, string text)
        {
            Guard.AgainstDefault(chatRoomId, nameof(chatRoomId));
            Guard.AgainstDefault(senderId, nameof(senderId));
            Guard.AgainstNullOrEmpty(text, nameof(text));

            ChatRoomId = chatRoomId;
            SenderId = senderId;
            SenderName = senderName;
            Text = text;
            Timestamp = DateTime.UtcNow;
            IsRead = false;
        }

        public void MarkAsRead()
        {
            if (!IsRead)
            {
                IsRead = true;
            }
        }
    }
}
