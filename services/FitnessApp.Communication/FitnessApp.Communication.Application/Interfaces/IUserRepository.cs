using FitnessApp.Communication.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FitnessApp.Communication.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<CommunicationUser?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<List<CommunicationUser>> GetCoachesAsync(CancellationToken ct);
        Task<List<CommunicationUser>> GetClientsAsync(CancellationToken ct);
        Task<List<CommunicationUser>> GetAllActiveAsync(CancellationToken ct);
        Task<List<CommunicationUser>> GetAllAsync(CancellationToken ct);
        Task AddAsync(CommunicationUser user, CancellationToken ct);
        Task UpdateAsync(CommunicationUser user, CancellationToken ct);
        Task<int> SaveChangesAsync(CancellationToken ct);
    }
}
