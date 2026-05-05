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
    public class UserRepository : IUserRepository
    {
        private readonly CommunicationDbContext _context;

        public UserRepository(CommunicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CommunicationUser user, CancellationToken ct)
        {
            await _context.Users.AddAsync(user, ct);
        }


        public async Task<List<CommunicationUser>> GetAllActiveAsync(CancellationToken ct)
        {
            return await _context.Users.Where(u => u.IsActive).ToListAsync(ct);
        }

        public async Task<List<CommunicationUser>> GetAllAsync(CancellationToken ct)
        {
            return await _context.Users.ToListAsync(ct);
        }

        public async Task<CommunicationUser?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _context.Users.FindAsync(new object[] { id }, ct);
        }

        public async Task<List<CommunicationUser>> GetCoachesAsync(CancellationToken ct)
        {
            return await _context.Users
                .Where(u => u.Role == "Trainer" && u.IsActive)
                .ToListAsync(ct);
        }

        public async Task<List<CommunicationUser>> GetClientsAsync(CancellationToken ct)
        {
            return await _context.Users
                .Where(u => u.Role == "User" && u.IsActive)
                .ToListAsync(ct);
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct)
        {
            return await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(CommunicationUser user, CancellationToken ct)
        {
            _context.Users.Update(user);
            await Task.CompletedTask;
        }
    }
}
