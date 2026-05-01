using FitnessApp.Nutrition.Domain.Interfaces.Persistence;
using FitnessApp.Nutrition.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace FitnessApp.Nutrition.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly NutritionDbContext _context;
        private IDbContextTransaction? _transaction;

        private IMealLogRepository? _mealLogRepository;
        private IDailyTargetRepository? _dailyTargetRepository;
        private IMealPlanTemplateRepository? _templateRepository;
        private IWeightLogRepository? _weightLogRepository;
        private IUserTemplateQueueRepository? _queueRepository;

        public UnitOfWork(NutritionDbContext context)
        {
            _context = context;
        }

        public IMealLogRepository MealLogs
        {
            get
            {
                _mealLogRepository ??= new MealLogRepository(_context);
                return _mealLogRepository;
            }
        }

        public IDailyTargetRepository DailyTargets
        {
            get
            {
                _dailyTargetRepository ??= new DailyTargetRepository(_context);
                return _dailyTargetRepository;
            }
        }

        public IMealPlanTemplateRepository Templates
        {
            get
            {
                _templateRepository ??= new MealPlanTemplateRepository(_context);
                return _templateRepository;
            }
        }

        public IWeightLogRepository WeightLogs
        {
            get
            {
                _weightLogRepository ??= new WeightLogRepository(_context);
                return _weightLogRepository;
            }
        }

        public IUserTemplateQueueRepository TemplateQueues
        {
            get
            {
                _queueRepository ??= new UserTemplateQueueRepository(_context);
                return _queueRepository;
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _context.SaveChangesAsync(ct);
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                await _transaction?.CommitAsync()!;
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                if (_transaction is not null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackAsync()
        {
            try
            {
                if (_transaction is not null)
                {
                    await _transaction.RollbackAsync();
                }
            }
            finally
            {
                if (_transaction is not null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
