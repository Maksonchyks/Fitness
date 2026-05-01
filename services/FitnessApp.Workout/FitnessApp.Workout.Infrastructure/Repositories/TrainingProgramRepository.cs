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
    public class TrainingProgramRepository : ITrainingProgramRepository
    {
        private readonly ApplicationDbContext _context;

        public TrainingProgramRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TrainingProgram program, CancellationToken ct = default)
        {
            await _context.TrainingPrograms.AddAsync(program, ct);
        }

        public async Task<TrainingProgram?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.TrainingPrograms
                .FirstOrDefaultAsync(p => p.Id == id, ct);
        }

        public async Task<IEnumerable<TrainingProgram>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.TrainingPrograms
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedOn)
                .ToListAsync(ct);
        }

        public void Remove(TrainingProgram program)
        {
            _context.TrainingPrograms.Remove(program);
        }
    }
}
