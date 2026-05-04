using System;
using System.Text.Json.Serialization;

namespace FitnessApp.Identity.Application.Common.Messaging
{
    public record UserCreatedEvent
    {
        [JsonPropertyName("userId")]
        public Guid UserId { get; init; }
        
        [JsonPropertyName("email")]
        public string Email { get; init; } = string.Empty;
        
        [JsonPropertyName("fullName")]
        public string FullName { get; init; } = string.Empty;
        
        [JsonPropertyName("role")]
        public string Role { get; init; } = "User";
    }
}
