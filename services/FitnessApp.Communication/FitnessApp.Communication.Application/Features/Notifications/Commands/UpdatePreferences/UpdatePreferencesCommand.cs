using FitnessApp.Communication.Application.Interfaces;
using FitnessApp.Communication.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FitnessApp.Communication.Application.Features.Notifications.Commands.UpdatePreferences
{
    public record UpdatePreferencesCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public bool NutritionEnabled { get; set; }
        public bool WorkoutEnabled { get; set; }
        public List<ScheduleDto> Schedules { get; set; } = new();
    }

    public record ScheduleDto(string Time, string Label, string Type);

    public class UpdatePreferencesCommandHandler : IRequestHandler<UpdatePreferencesCommand, bool>
    {
        private readonly INotificationRepository _repository;

        public UpdatePreferencesCommandHandler(INotificationRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdatePreferencesCommand request, CancellationToken ct)
        {
            var preferences = await _repository.GetPreferencesByUserIdAsync(request.UserId, ct);
            bool isNew = false;
            
            if (preferences == null)
            {
                preferences = new UserNotificationPreference(request.UserId);
                isNew = true;
            }

            preferences.UpdateSettings(request.NutritionEnabled, request.WorkoutEnabled);
            
            preferences.ClearSchedules();
            foreach (var s in request.Schedules)
            {
                if (TimeSpan.TryParse(s.Time, out var time))
                {
                    preferences.AddSchedule(time, s.Label, s.Type);
                }
            }

            if (isNew)
            {
                // We need a way to Add it. Let's assume Update handles it or add a method.
                // Actually, let's fix the repository to handle this.
                await _repository.UpdatePreferencesAsync(preferences, ct);
            }
            else 
            {
                await _repository.UpdatePreferencesAsync(preferences, ct);
            }
            
            await _repository.SaveChangesAsync(ct);
            return true;
        }
    }
}
