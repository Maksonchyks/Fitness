using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Domain.Interfaces;

namespace FitnessApp.Identity.Domain.Events
{
    public class UserPasswordChangedDomainEvent : IDomainEvent
    {
        public Guid UserId { get; }
        public DateTime OccurredOn { get; }
        public DateTime ChangedAt { get; }

        public UserPasswordChangedDomainEvent(Guid userId, DateTime changedAt)
        {
            UserId = userId;
            ChangedAt = changedAt;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
