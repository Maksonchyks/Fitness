using System;
using FitnessApp.Communication.Domain.Common;

namespace FitnessApp.Communication.Domain.Entities
{
    public class Participant : Entity
    {
        public Guid ChatRoomId { get; private set; }
        public Guid UserId { get; private set; }
        public string Username { get; private set; } = string.Empty;
        public DateTime JoinedAt { get; private set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public ChatRoom? ChatRoom { get; private set; }

        public CommunicationUser? User { get; private set; }

        private Participant() { } // For EF

        public Participant(Guid chatRoomId, Guid userId, string username)
        {
            Guard.AgainstDefault(chatRoomId, nameof(chatRoomId));
            Guard.AgainstDefault(userId, nameof(userId));
            Guard.AgainstNullOrEmpty(username, nameof(username));

            ChatRoomId = chatRoomId;
            UserId = userId;
            Username = username;
            JoinedAt = DateTime.UtcNow;
        }
    }
}
