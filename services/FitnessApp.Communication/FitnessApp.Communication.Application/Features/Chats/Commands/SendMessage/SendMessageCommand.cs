using FitnessApp.Communication.Application.Interfaces;
using FitnessApp.Communication.Domain.Entities;
using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FitnessApp.Communication.Application.Features.Chats.Commands.SendMessage
{
    public class SendMessageCommand : IRequest<ChatMessage>
    {
        public Guid RoomId { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;

        public SendMessageCommand() { }

        public SendMessageCommand(Guid roomId, Guid senderId, string senderName, string text)
        {
            RoomId = roomId;
            SenderId = senderId;
            SenderName = senderName;
            Text = text;
        }
    }

    public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
    {
        public SendMessageCommandValidator()
        {
            RuleFor(x => x.RoomId).NotEmpty();
            RuleFor(x => x.SenderId).NotEmpty();
            RuleFor(x => x.Text).NotEmpty().MaximumLength(2000);
            RuleFor(x => x.SenderName).NotEmpty();
        }
    }

    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, ChatMessage>
    {
        private readonly IChatRepository _chatRepository;
        private readonly INotificationService _notificationService;

        public SendMessageCommandHandler(IChatRepository chatRepository, INotificationService notificationService)
        {
            _chatRepository = chatRepository;
            _notificationService = notificationService;
        }

        public async Task<ChatMessage> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            var chatRoom = await _chatRepository.GetRoomByIdAsync(request.RoomId, cancellationToken);
            if (chatRoom == null)
            {
                throw new Exception("Chat room not found."); // We'll add better exception handling later
            }

            var message = chatRoom.AddMessage(request.SenderId, request.SenderName, request.Text);

            await _chatRepository.AddMessageAsync(message, cancellationToken);
            await _chatRepository.SaveChangesAsync(cancellationToken);

            // Notify via SignalR
            await _notificationService.NotifyNewMessageAsync(message);

            return message;
        }
    }
}
