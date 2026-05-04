using FitnessApp.Communication.Application.Interfaces;
using FitnessApp.Communication.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FitnessApp.Communication.Application.Features.Chats.Queries.GetRooms
{
    public record GetRoomsQuery(Guid UserId) : IRequest<List<ChatRoom>>;

    public class GetRoomsQueryHandler : IRequestHandler<GetRoomsQuery, List<ChatRoom>>
    {
        private readonly IChatRepository _chatRepository;

        public GetRoomsQueryHandler(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task<List<ChatRoom>> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
        {
            return await _chatRepository.GetUserRoomsAsync(request.UserId, cancellationToken);
        }
    }
}
