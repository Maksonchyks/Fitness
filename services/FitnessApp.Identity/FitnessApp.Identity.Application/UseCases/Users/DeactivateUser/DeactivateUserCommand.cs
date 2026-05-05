using System;
using System.Threading;
using System.Threading.Tasks;
using FitnessApp.Identity.Application.Common.Exceptions;
using FitnessApp.Identity.Domain.Interfaces.Repositories;
using MediatR;

namespace FitnessApp.Identity.Application.UseCases.Users.DeactivateUser
{
    public record DeactivateUserCommand(Guid UserId) : IRequest;

    public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly MassTransit.IPublishEndpoint _publishEndpoint;

        public DeactivateUserCommandHandler(IUserRepository userRepository, MassTransit.IPublishEndpoint publishEndpoint)
        {
            _userRepository = userRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new NotFoundException("User", request.UserId);
            }

            user.Deactivate();
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync(cancellationToken);

            await _publishEndpoint.Publish(new FitnessApp.Identity.Application.Common.Messaging.UserStatusChangedEvent
            {
                UserId = user.Id,
                IsActive = false
            }, cancellationToken);
        }
    }
}
