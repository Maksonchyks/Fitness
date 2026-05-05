using FitnessApp.Communication.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FitnessApp.Communication.Application.Interfaces
{
    public interface INotificationRepository
    {
        Task<UserNotificationPreference?> GetPreferencesByUserIdAsync(Guid userId, CancellationToken ct);
        Task<UserNotificationPreference?> GetPreferencesWithSchedulesByUserIdAsync(Guid userId, CancellationToken ct);
        Task AddScheduleAsync(ReminderSchedule schedule, CancellationToken ct);
        Task UpdatePreferencesAsync(UserNotificationPreference preferences, CancellationToken ct);
        Task AddNotificationAsync(UserNotification notification, CancellationToken ct);
        Task<List<UserNotification>> GetUserHistoryAsync(Guid userId, CancellationToken ct);
        Task<UserNotification?> GetNotificationByIdAsync(Guid id, CancellationToken ct);
        Task<List<ReminderSchedule>> GetSchedulesByTimeAsync(TimeSpan time, CancellationToken ct);
        Task ClearSchedulesAsync(Guid preferenceId, CancellationToken ct);
        Task DeletePreferencesByUserIdAsync(Guid userId, CancellationToken ct);
        Task<int> SaveChangesAsync(CancellationToken ct);
    }
}
