using FitnessApp.Nutrition.Domain.Entities;

namespace FitnessApp.Nutrition.Domain.Interfaces.Persistence
{
    public interface IUserTemplateQueueRepository
    {
        Task<UserTemplateQueue?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task AddAsync(UserTemplateQueue queue, CancellationToken ct = default);
    }
}
