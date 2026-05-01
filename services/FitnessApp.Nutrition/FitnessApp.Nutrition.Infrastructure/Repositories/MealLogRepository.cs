using FitnessApp.Nutrition.Domain.Entities;
using FitnessApp.Nutrition.Domain.Interfaces.Persistence;
using FitnessApp.Nutrition.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Nutrition.Infrastructure.Repositories
{
    public class MealLogRepository : IMealLogRepository
    {
        private readonly NutritionDbContext _context;

        public MealLogRepository(NutritionDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(MealLog mealLog, CancellationToken ct = default)
        {
            await _context.MealLogs.AddAsync(mealLog, ct);
        }

        public async Task<MealLog?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.MealLogs.FirstOrDefaultAsync(m => m.Id == id, ct);
        }

        public async Task<IEnumerable<MealLog>> GetByUserIdAndDateAsync(Guid userId, DateTime date, CancellationToken ct = default)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            return await _context.MealLogs
                .Where(m => m.UserId == userId && m.LoggedAt >= startOfDay && m.LoggedAt < endOfDay)
                .OrderBy(m => m.LoggedAt)
                .ToListAsync(ct);
        }

        public void Remove(MealLog mealLog)
        {
            _context.MealLogs.Remove(mealLog);
        }
    }
}
