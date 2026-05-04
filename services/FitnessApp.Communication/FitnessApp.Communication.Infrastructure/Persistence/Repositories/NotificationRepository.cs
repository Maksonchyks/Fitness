using FitnessApp.Communication.Application.Interfaces;
using FitnessApp.Communication.Domain.Entities;
using FitnessApp.Communication.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FitnessApp.Communication.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly CommunicationDbContext _context;

        public NotificationRepository(CommunicationDbContext context)
        {
            _context = context;
        }

        public async Task AddNotificationAsync(UserNotification notification, CancellationToken ct)
        {
            await _context.UserNotifications.AddAsync(notification, ct);
        }

        public async Task<UserNotification?> GetNotificationByIdAsync(Guid id, CancellationToken ct)
        {
            return await _context.UserNotifications.FindAsync(new object[] { id }, ct);
        }

        public async Task<UserNotificationPreference?> GetPreferencesByUserIdAsync(Guid userId, CancellationToken ct)
        {
            return await _context.NotificationPreferences
                .Include(p => p.Schedules)
                .FirstOrDefaultAsync(p => p.UserId == userId, ct);
        }

        public async Task<List<ReminderSchedule>> GetSchedulesByTimeAsync(TimeSpan time, CancellationToken ct)
        {
            return await _context.ReminderSchedules
                .Include(s => s.Preference)
                .Where(s => s.ReminderTime == time)
                .ToListAsync(ct);
        }

        public async Task<List<UserNotification>> GetUserHistoryAsync(Guid userId, CancellationToken ct)
        {
            return await _context.UserNotifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct)
        {
            return await _context.SaveChangesAsync(ct);
        }

        public async Task UpdatePreferencesAsync(UserNotificationPreference preferences, CancellationToken ct)
        {
            var entry = _context.Entry(preferences);
            if (entry.State == EntityState.Detached)
            {
                await _context.NotificationPreferences.AddAsync(preferences, ct);
            }
        }

        public async Task ClearSchedulesAsync(Guid preferenceId, CancellationToken ct)
        {
            var schedules = await _context.ReminderSchedules
                .Where(s => s.PreferenceId == preferenceId)
                .ToListAsync(ct);
            
            _context.ReminderSchedules.RemoveRange(schedules);
        }

        public async Task DeletePreferencesByUserIdAsync(Guid userId, CancellationToken ct)
        {
            var preferences = await _context.NotificationPreferences
                .Where(p => p.UserId == userId)
                .ToListAsync(ct);
            
            foreach(var pref in preferences)
            {
                await ClearSchedulesAsync(pref.Id, ct);
            }

            _context.NotificationPreferences.RemoveRange(preferences);
        }
    }
}
