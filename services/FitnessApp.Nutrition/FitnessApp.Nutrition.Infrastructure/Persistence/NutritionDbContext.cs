using FitnessApp.Nutrition.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Nutrition.Infrastructure.Persistence
{
    public class NutritionDbContext : DbContext
    {
        public NutritionDbContext(DbContextOptions<NutritionDbContext> options) : base(options) { }

        public DbSet<MealLog> MealLogs => Set<MealLog>();
        public DbSet<DailyTarget> DailyTargets => Set<DailyTarget>();
        public DbSet<MealPlanTemplate> MealPlanTemplates => Set<MealPlanTemplate>();
        public DbSet<TemplateMeal> TemplateMeals => Set<TemplateMeal>();
        public DbSet<WeightLog> WeightLogs => Set<WeightLog>();
        public DbSet<UserTemplateQueue> UserTemplateQueues => Set<UserTemplateQueue>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(NutritionDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
