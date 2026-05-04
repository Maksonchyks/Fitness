using System;
using System.Text.Json.Serialization;

namespace FitnessApp.Identity.Application.Common.Messaging
{
    public record UserStatusChangedEvent
    {
        [JsonPropertyName("userId")]
        public Guid UserId { get; init; }
        
        [JsonPropertyName("isActive")]
        public bool IsActive { get; init; }
    }
}
