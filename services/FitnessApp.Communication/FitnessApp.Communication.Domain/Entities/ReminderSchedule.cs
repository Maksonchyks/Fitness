using System;
using FitnessApp.Communication.Domain.Common;

namespace FitnessApp.Communication.Domain.Entities
{
    public class ReminderSchedule : Entity
    {
        public Guid PreferenceId { get; private set; }
        public TimeSpan ReminderTime { get; private set; }
        public string Label { get; private set; } = string.Empty;
        public string Type { get; private set; } = "General"; // Nutrition, Workout, etc.

        public UserNotificationPreference? Preference { get; private set; }

        private ReminderSchedule() { } // For EF

        public ReminderSchedule(Guid preferenceId, TimeSpan reminderTime, string label, string type = "General")
        {
            Guard.AgainstDefault(preferenceId, nameof(preferenceId));
            Guard.AgainstNullOrEmpty(label, nameof(label));

            PreferenceId = preferenceId;
            ReminderTime = reminderTime;
            Label = label;
            Type = type;
        }
    }
}
