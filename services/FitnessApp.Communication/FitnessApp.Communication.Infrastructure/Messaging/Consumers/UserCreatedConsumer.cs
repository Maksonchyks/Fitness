using FitnessApp.Communication.Application.Interfaces;
using FitnessApp.Communication.Domain.Entities;
using FitnessApp.Identity.Application.Common.Messaging;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FitnessApp.Communication.Infrastructure.Messaging.Consumers
{
    public class UserCreatedConsumer : IConsumer<UserCreatedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserCreatedConsumer> _logger;

        public UserCreatedConsumer(IUserRepository userRepository, ILogger<UserCreatedConsumer> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            try 
            {
                var message = context.Message;
                _logger.LogInformation("Consuming UserCreatedEvent for {Email}. ID: {UserId}", message.Email, message.UserId);

                var existingUser = await _userRepository.GetByIdAsync(message.UserId, context.CancellationToken);
                if (existingUser != null)
                {
                    _logger.LogWarning("User {UserId} already exists in Communication cache.", message.UserId);
                    return;
                }

                // Using the new DDD constructor
                var user = new CommunicationUser(
                    message.UserId,
                    message.Email,
                    message.Email,
                    message.FullName,
                    message.Role
                );

                await _userRepository.AddAsync(user, context.CancellationToken);
                await _userRepository.SaveChangesAsync(context.CancellationToken);
                
                _logger.LogInformation("User {UserId} ({Email}) synced to Communication service.", user.Id, user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while consuming UserCreatedEvent");
                throw; // Rethrow to let MassTransit handle the failure (move to error queue)
            }
        }
    }
}
