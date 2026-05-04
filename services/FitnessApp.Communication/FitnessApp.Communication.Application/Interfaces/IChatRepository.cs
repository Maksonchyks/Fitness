using FitnessApp.Communication.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FitnessApp.Communication.Application.Interfaces
{
    public interface IChatRepository
    {
        Task<ChatRoom?> GetRoomByIdAsync(Guid id, CancellationToken ct);
        Task<List<ChatRoom>> GetUserRoomsAsync(Guid userId, CancellationToken ct);
        Task CreateRoomAsync(ChatRoom room, CancellationToken ct);
        Task AddMessageAsync(ChatMessage message, CancellationToken ct);
        Task<List<ChatMessage>> GetRoomMessagesAsync(Guid roomId, CancellationToken ct);
        Task<int> SaveChangesAsync(CancellationToken ct);
    }
}
