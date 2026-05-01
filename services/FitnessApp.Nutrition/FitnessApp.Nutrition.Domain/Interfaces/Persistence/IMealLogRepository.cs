using FitnessApp.Nutrition.Domain.Entities;

namespace FitnessApp.Nutrition.Domain.Interfaces.Persistence
{
    public interface IMealLogRepository
    {
        Task AddAsync(MealLog mealLog, CancellationToken ct = default);
        Task<MealLog?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<MealLog>> GetByUserIdAndDateAsync(Guid userId, DateTime date, CancellationToken ct = default);
        void Remove(MealLog mealLog);
    }
}
