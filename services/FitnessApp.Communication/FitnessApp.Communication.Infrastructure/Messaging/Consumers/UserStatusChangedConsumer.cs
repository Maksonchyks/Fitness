using FitnessApp.Communication.Application.Interfaces;
using FitnessApp.Identity.Application.Common.Messaging;
using MassTransit;
using System.Threading.Tasks;

namespace FitnessApp.Communication.Infrastructure.Messaging.Consumers
{
    public class UserStatusChangedConsumer : IConsumer<UserStatusChangedEvent>
    {
        private readonly IUserRepository _userRepository;

        public UserStatusChangedConsumer(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Consume(ConsumeContext<UserStatusChangedEvent> context)
        {
            var message = context.Message;
            var user = await _userRepository.GetByIdAsync(message.UserId, context.CancellationToken);

            if (user != null)
            {
                user.SetActiveStatus(message.IsActive);
                await _userRepository.UpdateAsync(user, context.CancellationToken);
                await _userRepository.SaveChangesAsync(context.CancellationToken);
            }
        }
    }
}
