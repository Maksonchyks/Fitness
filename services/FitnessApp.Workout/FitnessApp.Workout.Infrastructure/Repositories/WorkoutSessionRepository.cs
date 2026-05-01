using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Domain.Entities;
using FitnessApp.Workout.Domain.Interfaces.Persistence;
using FitnessApp.Workout.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Workout.Infrastructure.Repositories
{
    public class WorkoutSessionRepository : IWorkoutSessionRepository
    {
        private readonly ApplicationDbContext _context;

        public WorkoutSessionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(WorkoutSession session, CancellationToken ct = default)
        {
            await _context.WorkoutSessions.AddAsync(session, ct);
        }

        public async Task<WorkoutSession?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.WorkoutSessions
                .FirstOrDefaultAsync(s => s.Id == id, ct);
        }

        public async Task<IEnumerable<WorkoutSession>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.WorkoutSessions
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.Date)
                .ToListAsync(ct);
        }

        public void Update(WorkoutSession session)
        {
            _context.WorkoutSessions.Update(session);
        }

        public void Remove(WorkoutSession session)
        {
            _context.WorkoutSessions.Remove(session);
        }
    }
}
