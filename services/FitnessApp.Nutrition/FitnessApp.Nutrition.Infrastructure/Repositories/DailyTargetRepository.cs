using FitnessApp.Nutrition.Domain.Entities;
using FitnessApp.Nutrition.Domain.Interfaces.Persistence;
using FitnessApp.Nutrition.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Nutrition.Infrastructure.Repositories
{
    public class DailyTargetRepository : IDailyTargetRepository
    {
        private readonly NutritionDbContext _context;

        public DailyTargetRepository(NutritionDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(DailyTarget target, CancellationToken ct = default)
        {
            await _context.DailyTargets.AddAsync(target, ct);
        }

        public async Task<DailyTarget?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.DailyTargets
                .FirstOrDefaultAsync(d => d.UserId == userId, ct);
        }
    }
}
