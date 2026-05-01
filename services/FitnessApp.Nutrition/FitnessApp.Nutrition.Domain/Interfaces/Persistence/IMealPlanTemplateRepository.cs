using FitnessApp.Nutrition.Domain.Entities;

namespace FitnessApp.Nutrition.Domain.Interfaces.Persistence
{
    public interface IMealPlanTemplateRepository
    {
        Task<IEnumerable<MealPlanTemplate>> GetAllAsync(CancellationToken ct = default);
        Task<MealPlanTemplate?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<int> GetCountAsync(CancellationToken ct = default);
        Task AddAsync(MealPlanTemplate template, CancellationToken ct = default);
    }
}
