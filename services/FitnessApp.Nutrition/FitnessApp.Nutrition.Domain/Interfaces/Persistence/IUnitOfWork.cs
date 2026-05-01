namespace FitnessApp.Nutrition.Domain.Interfaces.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        IMealLogRepository MealLogs { get; }
        IDailyTargetRepository DailyTargets { get; }
        IMealPlanTemplateRepository Templates { get; }
        IWeightLogRepository WeightLogs { get; }
        IUserTemplateQueueRepository TemplateQueues { get; }
        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
