using FitnessApp.Communication.Application.Interfaces;
using FitnessApp.Communication.Domain.Entities;
using FitnessApp.Identity.Application.Common.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FitnessApp.Communication.Infrastructure.Messaging.Consumers
{
    public class UserRoleChangedConsumer : IConsumer<UserRoleChangedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserRoleChangedConsumer> _logger;

        public UserRoleChangedConsumer(IUserRepository userRepository, ILogger<UserRoleChangedConsumer> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserRoleChangedEvent> context)
        {
            try 
            {
                var message = context.Message;
                _logger.LogInformation("Updating role for user {UserId} to {NewRole}. Email: {Email}", message.UserId, message.NewRole, message.Email);

                var user = await _userRepository.GetByIdAsync(message.UserId, context.CancellationToken);
                if (user == null)
                {
                    _logger.LogInformation("User {UserId} not found in Communication cache. Creating new user record.", message.UserId);
                    user = new CommunicationUser(
                        message.UserId,
                        message.Email,
                        message.Email,
                        message.FullName,
                        message.NewRole
                    );
                    await _userRepository.AddAsync(user, context.CancellationToken);
                }
                else
                {
                    user.Role = message.NewRole;
                }

                await _userRepository.SaveChangesAsync(context.CancellationToken);
                _logger.LogInformation("User {UserId} role/info updated successfully.", user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while consuming UserRoleChangedEvent for {UserId}", context.Message.UserId);
                throw;
            }
        }
    }
}
