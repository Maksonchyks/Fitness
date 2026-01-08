using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Domain.Entities;

namespace FitnessApp.Identity.Domain.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
        Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default);
        void Update(RefreshToken token);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
