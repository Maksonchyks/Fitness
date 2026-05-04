using FitnessApp.Communication.Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FitnessApp.Communication.Application.Features.Chats.Commands.MarkAsRead
{
    public record MarkAsReadCommand(Guid RoomId, Guid UserId) : IRequest<bool>;

    public class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand, bool>
    {
        private readonly IChatRepository _chatRepository;
        private readonly INotificationService _notificationService;

        public MarkAsReadCommandHandler(IChatRepository chatRepository, INotificationService notificationService)
        {
            _chatRepository = chatRepository;
            _notificationService = notificationService;
        }

        public async Task<bool> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
        {
            var messages = await _chatRepository.GetRoomMessagesAsync(request.RoomId, cancellationToken);
            
            bool changed = false;
            Guid? senderIdToNotify = null;

            foreach (var message in messages)
            {
                if (message.SenderId != request.UserId && !message.IsRead)
                {
                    message.MarkAsRead();
                    changed = true;
                    senderIdToNotify = message.SenderId;
                }
            }

            if (changed)
            {
                await _chatRepository.SaveChangesAsync(cancellationToken);
                
                // Notify the sender that their messages were read
                if (senderIdToNotify.HasValue)
                {
                    await _notificationService.NotifyMessagesReadAsync(request.RoomId, request.UserId);
                }
                
                return true;
            }

            return false;
        }
    }
}
