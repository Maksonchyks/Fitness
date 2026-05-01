using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Domain.Interfaces.Persistence;
using FitnessApp.Workout.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace FitnessApp.Workout.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        private ITrainingProgramRepository? _programRepository;
        private IWorkoutSessionRepository? _sessionRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public ITrainingProgramRepository Programs
        {
            get
            {
                if (_programRepository is null)
                {
                    _programRepository = new TrainingProgramRepository(_context);
                }
                return _programRepository;
            }
        }

        public IWorkoutSessionRepository Sessions
        {
            get
            {
                if (_sessionRepository is null)
                {
                    _sessionRepository = new WorkoutSessionRepository(_context);
                }
                return _sessionRepository;
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
