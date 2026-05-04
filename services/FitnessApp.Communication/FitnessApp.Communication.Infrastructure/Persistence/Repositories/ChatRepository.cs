using FitnessApp.Communication.Application.Interfaces;
using FitnessApp.Communication.Domain.Entities;
using FitnessApp.Communication.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FitnessApp.Communication.Infrastructure.Persistence.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly CommunicationDbContext _context;

        public ChatRepository(CommunicationDbContext context)
        {
            _context = context;
        }

        public async Task AddMessageAsync(ChatMessage message, CancellationToken ct)
        {
            await _context.ChatMessages.AddAsync(message, ct);
        }

        public async Task CreateRoomAsync(ChatRoom room, CancellationToken ct)
        {
            await _context.ChatRooms.AddAsync(room, ct);
        }

        public async Task<ChatRoom?> GetRoomByIdAsync(Guid id, CancellationToken ct)
        {
            return await _context.ChatRooms
                .Include(r => r.Participants)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(r => r.Id == id, ct);
        }

        public async Task<List<ChatMessage>> GetRoomMessagesAsync(Guid roomId, CancellationToken ct)
        {
            return await _context.ChatMessages
                .Where(m => m.ChatRoomId == roomId)
                .OrderBy(m => m.Timestamp)
                .ToListAsync(ct);
        }

        public async Task<List<ChatRoom>> GetUserRoomsAsync(Guid userId, CancellationToken ct)
        {
            return await _context.ChatRooms
                .Include(r => r.Participants)
                    .ThenInclude(p => p.User)
                .Where(r => r.Participants.Any(p => p.UserId == userId))
                .ToListAsync(ct);
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct)
        {
            return await _context.SaveChangesAsync(ct);
        }
    }
}
