using FitnessApp.Nutrition.Domain.Entities;
using FitnessApp.Nutrition.Domain.Interfaces.Persistence;
using FitnessApp.Nutrition.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Nutrition.Infrastructure.Repositories
{
    public class WeightLogRepository : IWeightLogRepository
    {
        private readonly NutritionDbContext _context;

        public WeightLogRepository(NutritionDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(WeightLog log, CancellationToken ct = default)
        {
            await _context.WeightLogs.AddAsync(log, ct);
        }

        public async Task<IEnumerable<WeightLog>> GetByUserIdAsync(Guid userId, int days, CancellationToken ct = default)
        {
            var since = DateTime.UtcNow.AddDays(-days);

            return await _context.WeightLogs
                .Where(w => w.UserId == userId && w.LoggedAt >= since)
                .OrderByDescending(w => w.LoggedAt)
                .ToListAsync(ct);
        }
    }
}
