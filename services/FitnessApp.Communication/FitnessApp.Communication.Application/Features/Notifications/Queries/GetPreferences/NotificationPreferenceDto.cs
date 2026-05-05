using System;
using System.Collections.Generic;

namespace FitnessApp.Communication.Application.Features.Notifications.Queries.GetPreferences
{
    public class NotificationPreferenceDto
    {
        public Guid UserId { get; set; }
        public bool NutritionEnabled { get; set; }
        public bool WorkoutEnabled { get; set; }
        public List<ScheduleDto> Schedules { get; set; } = new();
    }

    public class ScheduleDto
    {
        public TimeSpan ReminderTime { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
    }
}
