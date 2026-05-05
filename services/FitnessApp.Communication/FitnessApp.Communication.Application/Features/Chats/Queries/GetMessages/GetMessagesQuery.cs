using FitnessApp.Communication.Application.Interfaces;
using FitnessApp.Communication.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FitnessApp.Communication.Application.Features.Chats.Queries.GetMessages
{
    public record GetMessagesQuery(Guid RoomId) : IRequest<List<ChatMessage>>;

    public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, List<ChatMessage>>
    {
        private readonly IChatRepository _chatRepository;

        public GetMessagesQueryHandler(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task<List<ChatMessage>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
        {
            return await _chatRepository.GetRoomMessagesAsync(request.RoomId, cancellationToken);
        }
    }
}
