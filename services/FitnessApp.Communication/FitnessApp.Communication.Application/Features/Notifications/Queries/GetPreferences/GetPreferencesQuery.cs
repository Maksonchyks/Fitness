using FitnessApp.Communication.Application.Interfaces;
using FitnessApp.Communication.Domain.Entities;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FitnessApp.Communication.Application.Features.Notifications.Queries.GetPreferences
{
    public record GetPreferencesQuery(Guid UserId) : IRequest<NotificationPreferenceDto>;

    public class GetPreferencesQueryHandler : IRequestHandler<GetPreferencesQuery, NotificationPreferenceDto>
    {
        private readonly INotificationRepository _notificationRepository;

        public GetPreferencesQueryHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<NotificationPreferenceDto> Handle(GetPreferencesQuery request, CancellationToken cancellationToken)
        {
            var pref = await _notificationRepository.GetPreferencesWithSchedulesByUserIdAsync(request.UserId, cancellationToken);

            if (pref == null)
            {
                return new NotificationPreferenceDto
                {
                    UserId = request.UserId,
                    NutritionEnabled = true,
                    WorkoutEnabled = true,
                    Schedules = new()
                };
            }

            return new NotificationPreferenceDto
            {
                UserId = pref.UserId,
                NutritionEnabled = pref.NutritionEnabled,
                WorkoutEnabled = pref.WorkoutEnabled,
                Schedules = pref.Schedules.Select(s => new ScheduleDto
                {
                    ReminderTime = s.ReminderTime,
                    Label = s.Label,
                    Type = s.Type
                }).ToList()
            };
        }
    }
}
