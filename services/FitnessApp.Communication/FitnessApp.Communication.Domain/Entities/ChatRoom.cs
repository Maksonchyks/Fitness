using System;
using System.Collections.Generic;
using FitnessApp.Communication.Domain.Common;

namespace FitnessApp.Communication.Domain.Entities
{
    public class ChatRoom : AggregateRoot
    {
        public string? Name { get; private set; }
        public DateTime CreatedAt { get; private set; }
        
        private readonly List<Participant> _participants = new();
        public IReadOnlyCollection<Participant> Participants => _participants.AsReadOnly();

        private readonly List<ChatMessage> _messages = new();
        public IReadOnlyCollection<ChatMessage> Messages => _messages.AsReadOnly();

        private ChatRoom() { } // For EF

        public ChatRoom(string? name = null)
        {
            Name = name;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddParticipant(Guid userId, string username)
        {
            if (!_participants.Exists(p => p.UserId == userId))
            {
                _participants.Add(new Participant(Id, userId, username));
            }
        }

        public ChatMessage AddMessage(Guid senderId, string senderName, string text)
        {
            var message = new ChatMessage(Id, senderId, senderName, text);
            _messages.Add(message);
            
            // Here we could add a domain event if needed
            // AddDomainEvent(new MessageSentEvent(message));
            
            return message;
        }
    }
}
