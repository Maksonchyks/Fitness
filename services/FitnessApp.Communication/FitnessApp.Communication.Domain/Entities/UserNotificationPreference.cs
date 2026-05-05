using System;
using System.Collections.Generic;
using FitnessApp.Communication.Domain.Common;

namespace FitnessApp.Communication.Domain.Entities
{
    public class UserNotificationPreference : AggregateRoot
    {
        public Guid UserId { get; private set; }
        public bool NutritionEnabled { get; private set; } = true;
        public bool WorkoutEnabled { get; private set; } = true;

        private readonly List<ReminderSchedule> _schedules = new();
        public IReadOnlyCollection<ReminderSchedule> Schedules => _schedules.AsReadOnly();

        private UserNotificationPreference() { } // For EF

        public UserNotificationPreference(Guid userId)
        {
            Guard.AgainstDefault(userId, nameof(userId));
            UserId = userId;
        }

        public void UpdateSettings(bool nutritionEnabled, bool workoutEnabled)
        {
            NutritionEnabled = nutritionEnabled;
            WorkoutEnabled = workoutEnabled;
        }

        public void AddSchedule(TimeSpan reminderTime, string label, string type)
        {
            _schedules.Add(new ReminderSchedule(Id, reminderTime, label, type));
        }

        public void ClearSchedules()
        {
            _schedules.Clear();
        }
    }
}
