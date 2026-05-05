using FitnessApp.Communication.Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FitnessApp.Communication.Application.Features.Users.Commands.UpdateUserStatus
{
    public record UpdateUserStatusCommand(Guid UserId, bool IsOnline) : IRequest;

    public class UpdateUserStatusCommandHandler : IRequestHandler<UpdateUserStatusCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;

        public UpdateUserStatusCommandHandler(IUserRepository userRepository, INotificationService notificationService)
        {
            _userRepository = userRepository;
            _notificationService = notificationService;
        }

        public async Task Handle(UpdateUserStatusCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user != null)
            {
                user.SetOnlineStatus(request.IsOnline);
                await _userRepository.UpdateAsync(user, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);
                
                await _notificationService.NotifyUserStatusChangedAsync(request.UserId, request.IsOnline);
            }
        }
    }
}
