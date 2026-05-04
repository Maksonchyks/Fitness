using FitnessApp.Communication.Application.Features.Notifications.Commands.ProcessReminders;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FitnessApp.Communication.Infrastructure.BackgroundServices
{
    public class ReminderBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReminderBackgroundService> _logger;
        private int _lastProcessedMinute = -1;

        public ReminderBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<ReminderBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Reminder Background Service is starting.");

            // Poll every 10 seconds to ensure we don't skip minutes and trigger close to the minute start.
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    var now = DateTime.Now; // Keeping local time as requested by the current setup
                    
                    if (now.Minute == _lastProcessedMinute)
                        continue; // Already processed reminders for this minute

                    _lastProcessedMinute = now.Minute;

                    using var scope = _serviceProvider.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    
                    await mediator.Send(new ProcessRemindersCommand(now), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing reminders.");
                }
            }
        }
    }
}
