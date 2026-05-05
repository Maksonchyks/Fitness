using FitnessApp.Communication.Application.Interfaces;
using FitnessApp.Communication.Domain.Entities;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FitnessApp.Communication.Application.Features.Notifications.Commands.ProcessReminders
{
    public record ProcessRemindersCommand(DateTime CurrentTime) : IRequest;

    public class ProcessRemindersCommandHandler : IRequestHandler<ProcessRemindersCommand>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ProcessRemindersCommandHandler> _logger;

        public ProcessRemindersCommandHandler(
            INotificationRepository notificationRepository, 
            INotificationService notificationService,
            ILogger<ProcessRemindersCommandHandler> logger)
        {
            _notificationRepository = notificationRepository;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Handle(ProcessRemindersCommand request, CancellationToken cancellationToken)
        {
            // Floor time to minutes
            var timeSpan = new TimeSpan(request.CurrentTime.Hour, request.CurrentTime.Minute, 0);
            _logger.LogInformation("Background Service: Checking reminders for time {Time}", timeSpan);
            
            var schedules = await _notificationRepository.GetSchedulesByTimeAsync(timeSpan, cancellationToken);
            _logger.LogInformation("Found {Count} schedules for time {Time}", schedules.Count, timeSpan);

            foreach (var schedule in schedules)
            {
                if (schedule.Preference == null) 
                {
                    _logger.LogWarning("Preference is null for schedule {ScheduleId}", schedule.Id);
                    continue;
                }

                // Check if global settings allow this type
                bool isEnabled = schedule.Type == "Nutrition" ? schedule.Preference.NutritionEnabled : schedule.Preference.WorkoutEnabled;
                _logger.LogInformation("Schedule {ScheduleId} isEnabled: {IsEnabled}", schedule.Id, isEnabled);
                
                if (isEnabled)
                {
                    var notification = new UserNotification(
                        schedule.Preference.UserId,
                        schedule.Label,
                        $"Reminder for your {schedule.Type} task.",
                        schedule.Type
                    );

                    await _notificationRepository.AddNotificationAsync(notification, cancellationToken);
                    
                    // Send real-time notification
                    await _notificationService.SendLiveReminderAsync(
                        schedule.Preference.UserId, 
                        notification.Title, 
                        notification.Message, 
                        notification.Type
                    );
                }
            }

            await _notificationRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
