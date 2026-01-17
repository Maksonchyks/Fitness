using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Domain.Entities;
using FitnessApp.Identity.Domain.Enums;
using FitnessApp.Identity.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Identity.Infrastructure.Data.Seed
{
    public class DatabaseSeeder
    {
        private readonly IdentityDbContext _context;

        public DatabaseSeeder(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await SeedRolesAsync();
            await SeedDefaultAdminAsync();
        }

        private async Task SeedRolesAsync()
        {
            if (await _context.Roles.AnyAsync())
                return;

            var roles = new List<Role>
        {
            new Role("User", "Regular user", true),
            new Role("Trainer", "Certified fitness trainer"),
            new Role("Admin", "System administrator")
        };

            await _context.Roles.AddRangeAsync(roles);
            await _context.SaveChangesAsync();
        }

        private async Task SeedDefaultAdminAsync()
        {
            // 1. Перевірка на існування (залишаємо як було)
            if (await _context.Users.AnyAsync(u => u.Username == "admin"))
                return;

            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            if (adminRole == null)
                return;

            var password = Password.Create(
                "$2a$11$abcdefghijklmnopqrstuv",
                "$2a$11$abcdefghijklmnopqrstuv"
            );

            var admin = User.Create(
                Email.Create("admin@fitnessapp.com"),
                "admin",
                password,
                "System",
                "Administrator"
            );

            admin.AddRole(adminRole);

            if (!admin.EmailConfirmed)
            {
                admin.ConfirmEmail();
            }

            if (admin.Status != AccountStatus.Active)
            {
                admin.Activate();
            }

            await _context.Users.AddAsync(admin);
            await _context.SaveChangesAsync();
        }
    }
}
