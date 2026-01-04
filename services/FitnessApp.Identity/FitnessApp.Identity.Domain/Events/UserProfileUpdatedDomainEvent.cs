using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Domain.Enums;
using FitnessApp.Identity.Domain.Interfaces;

namespace FitnessApp.Identity.Domain.Events
{
    public class UserProfileUpdatedDomainEvent : IDomainEvent
    {
        public Guid UserId { get; }
        public string? FirstName { get; }
        public string? LastName { get; }
        public DateTime DateOfBirth { get; }
        public Gender Gender { get; }
        public FitnessGoal FitnessGoal { get; }
        public DateTime OccurredOn { get; }

        public UserProfileUpdatedDomainEvent(
            Guid userId,
            string? firstName,
            string? lastName,
            DateTime dateOfBirth,
            Gender gender,
            FitnessGoal fitnessGoal)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            FitnessGoal = fitnessGoal;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
