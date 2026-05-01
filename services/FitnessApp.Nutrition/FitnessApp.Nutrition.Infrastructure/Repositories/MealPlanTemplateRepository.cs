using FitnessApp.Nutrition.Domain.Entities;
using FitnessApp.Nutrition.Domain.Interfaces.Persistence;
using FitnessApp.Nutrition.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Nutrition.Infrastructure.Repositories
{
    public class MealPlanTemplateRepository : IMealPlanTemplateRepository
    {
        private readonly NutritionDbContext _context;

        public MealPlanTemplateRepository(NutritionDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MealPlanTemplate>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.MealPlanTemplates
                .OrderBy(t => t.Name)
                .ToListAsync(ct);
        }

        public async Task<MealPlanTemplate?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.MealPlanTemplates
                .FirstOrDefaultAsync(t => t.Id == id, ct);
        }

        public async Task<int> GetCountAsync(CancellationToken ct = default)
        {
            return await _context.MealPlanTemplates.CountAsync(ct);
        }

        public async Task AddAsync(MealPlanTemplate template, CancellationToken ct = default)
        {
            await _context.MealPlanTemplates.AddAsync(template, ct);
        }
    }
}
