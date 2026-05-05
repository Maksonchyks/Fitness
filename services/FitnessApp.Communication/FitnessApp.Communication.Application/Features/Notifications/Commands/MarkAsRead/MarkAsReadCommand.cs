using FitnessApp.Communication.Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FitnessApp.Communication.Application.Features.Notifications.Commands.MarkAsRead
{
    public record MarkAsReadCommand(Guid NotificationId) : IRequest<bool>;

    public class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand, bool>
    {
        private readonly INotificationRepository _notificationRepository;

        public MarkAsReadCommandHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<bool> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
        {
            var notification = await _notificationRepository.GetNotificationByIdAsync(request.NotificationId, cancellationToken);
            if (notification == null) return false;

            notification.MarkAsRead();
            await _notificationRepository.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
