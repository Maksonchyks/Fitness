using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Domain.Interfaces.Repositories;
using FitnessApp.Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Identity.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IdentityDbContext _context;

        public RoleRepository(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
        }

        public async Task<List<Role>> GetDefaultRolesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .Where(r => r.IsDefault)
                .ToListAsync(cancellationToken);
        }
    }
}
