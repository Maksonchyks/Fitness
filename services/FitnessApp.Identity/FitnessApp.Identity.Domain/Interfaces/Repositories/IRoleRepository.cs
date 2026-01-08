using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Identity.Domain.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<List<Role>> GetDefaultRolesAsync(CancellationToken cancellationToken = default);
    }
}
