using FitnessApp.Communication.Application.Interfaces;
using FitnessApp.Communication.Domain.Entities;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FitnessApp.Communication.Application.Features.Chats.Commands.CreateChatRoom
{
    public class CreateChatRoomCommand : IRequest<ChatRoom>
    {
        public Guid CreatorId { get; set; }
        public Guid TargetUserId { get; set; }
        public string RoomName { get; set; } = string.Empty;

        public CreateChatRoomCommand() { }

        public CreateChatRoomCommand(Guid creatorId, Guid targetUserId, string roomName)
        {
            CreatorId = creatorId;
            TargetUserId = targetUserId;
            RoomName = roomName;
        }
    }

    public class CreateChatRoomCommandValidator : AbstractValidator<CreateChatRoomCommand>
    {
        public CreateChatRoomCommandValidator()
        {
            RuleFor(x => x.CreatorId).NotEmpty();
            RuleFor(x => x.TargetUserId).NotEmpty();
            RuleFor(x => x.RoomName).NotEmpty().MaximumLength(100);
        }
    }

    public class CreateChatRoomCommandHandler : IRequestHandler<CreateChatRoomCommand, ChatRoom>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;

        public CreateChatRoomCommandHandler(IChatRepository chatRepository, IUserRepository userRepository)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }

        public async Task<ChatRoom> Handle(CreateChatRoomCommand request, CancellationToken cancellationToken)
        {
            // 1. Check if private chat already exists
            var existingRooms = await _chatRepository.GetUserRoomsAsync(request.CreatorId, cancellationToken);
            
            var existingPrivateRoom = existingRooms.FirstOrDefault(r => 
                r.Participants.Any(p => p.UserId == request.TargetUserId) && 
                r.Participants.Count == 2);

            if (existingPrivateRoom != null)
            {
                return existingPrivateRoom;
            }

            // 2. Fetch usernames for participants
            var creator = await _userRepository.GetByIdAsync(request.CreatorId, cancellationToken);
            var target = await _userRepository.GetByIdAsync(request.TargetUserId, cancellationToken);

            if (creator == null || target == null)
            {
                throw new Exception("One or more users not found in communication service.");
            }

            // 3. Create new room using DDD domain methods
            var room = new ChatRoom(request.RoomName);
            room.AddParticipant(creator.Id, creator.FullName);
            room.AddParticipant(target.Id, target.FullName);

            await _chatRepository.CreateRoomAsync(room, cancellationToken);
            await _chatRepository.SaveChangesAsync(cancellationToken);

            return room;
        }
    }
}
