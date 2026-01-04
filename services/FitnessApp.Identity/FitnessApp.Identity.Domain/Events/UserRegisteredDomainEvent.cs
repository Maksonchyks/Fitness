using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Domain.Interfaces;

namespace FitnessApp.Identity.Domain.Events
{
    public class UserRegisteredDomainEvent : IDomainEvent
    {
        public Guid UserId { get; }
        public string Email { get; }
        public string Username { get; }
        public DateTime OccurredOn { get; }

        public UserRegisteredDomainEvent(Guid userId, string email, string username)
        {
            UserId = userId;
            Email = email;
            Username = username;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
