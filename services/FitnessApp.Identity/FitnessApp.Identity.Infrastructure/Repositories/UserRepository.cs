using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Domain.Entities;
using FitnessApp.Identity.Domain.Interfaces.Repositories;
using FitnessApp.Identity.Domain.ValueObjects;
using FitnessApp.Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Identity.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IdentityDbContext _context;
        public UserRepository(IdentityDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            await _context.Users.AddAsync(user, cancellationToken);
            
        }

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var emailObject = Email.Create(email);
            return await _context.Users
                .AnyAsync(u => u.Email == emailObject, cancellationToken);
        }

        public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            var lowerUsername = username.ToLower();
            return await _context.Users
                .AnyAsync(u => u.Username == lowerUsername, cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var emailObject = Email.Create(email);

            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == emailObject, cancellationToken);
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        }

        public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ToListAsync(cancellationToken);
        }

        public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task ClearUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            // 1. Очищуємо колекцію в пам'яті, якщо користувач відстежується
            var trackedUser = _context.ChangeTracker.Entries<User>()
                .FirstOrDefault(e => e.Entity.Id == userId);
            
            if (trackedUser != null)
            {
                trackedUser.Entity.ClearRoles();
            }

            // 2. Видаляємо з бази
            await _context.Set<UserRole>()
                .Where(ur => ur.UserId == userId)
                .ExecuteDeleteAsync(cancellationToken);

            // 3. Від'єднуємо застарілі об'єкти відстеження
            var trackedUserRoles = _context.ChangeTracker.Entries<UserRole>()
                .Where(e => e.Entity.UserId == userId)
                .ToList();
            foreach (var entry in trackedUserRoles)
                entry.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        public async Task AddUserRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
        {
            var userRole = await _context.Set<UserRole>()
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
            
            if (userRole == null)
            {
                // Використовуємо прямий insert, щоб уникнути конфліктів трекінгу
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO \"UserRoles\" (\"Id\", \"UserId\", \"RoleId\", \"AssignedAt\") VALUES ({0}, {1}, {2}, {3})",
                    new object[] { Guid.NewGuid(), userId, roleId, DateTime.UtcNow },
                    cancellationToken);
            }
        }

        public async Task<int> CountUsersInRoleAsync(string roleName, CancellationToken cancellationToken = default)
        {
            return await _context.UserRoles
                .CountAsync(ur => ur.Role!.Name == roleName, cancellationToken);
        }
    }
}
