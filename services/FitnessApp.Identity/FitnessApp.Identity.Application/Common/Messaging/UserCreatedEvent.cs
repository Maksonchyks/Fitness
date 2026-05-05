using System;

namespace FitnessApp.Identity.Application.Common.Messaging
{
    public record UserCreatedEvent
    {
        public Guid UserId { get; init; }
        public string Email { get; init; } = string.Empty;
        public string FullName { get; init; } = string.Empty;
        public string Role { get; init; } = "User";
    }
}
