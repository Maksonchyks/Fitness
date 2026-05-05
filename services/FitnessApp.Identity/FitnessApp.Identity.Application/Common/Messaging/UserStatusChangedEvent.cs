using System;

namespace FitnessApp.Identity.Application.Common.Messaging
{
    public record UserStatusChangedEvent
    {
        public Guid UserId { get; init; }
        public bool IsActive { get; init; }
    }
}
