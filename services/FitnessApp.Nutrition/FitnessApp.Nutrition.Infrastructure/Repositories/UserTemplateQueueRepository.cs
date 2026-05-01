using FitnessApp.Nutrition.Domain.Entities;
using FitnessApp.Nutrition.Domain.Interfaces.Persistence;
using FitnessApp.Nutrition.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Nutrition.Infrastructure.Repositories
{
    public class UserTemplateQueueRepository : IUserTemplateQueueRepository
    {
        private readonly NutritionDbContext _context;

        public UserTemplateQueueRepository(NutritionDbContext context)
        {
            _context = context;
        }

        public async Task<UserTemplateQueue?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.UserTemplateQueues
                .FirstOrDefaultAsync(q => q.UserId == userId, ct);
        }

        public async Task AddAsync(UserTemplateQueue queue, CancellationToken ct = default)
        {
            await _context.UserTemplateQueues.AddAsync(queue, ct);
        }
    }
}
