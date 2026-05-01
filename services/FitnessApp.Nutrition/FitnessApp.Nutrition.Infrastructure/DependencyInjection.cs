using FitnessApp.Nutrition.Application.Interfaces;
using FitnessApp.Nutrition.Domain.Interfaces.Persistence;
using FitnessApp.Nutrition.Infrastructure.Persistence;
using FitnessApp.Nutrition.Infrastructure.Repositories;
using FitnessApp.Nutrition.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitnessApp.Nutrition.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not set");

            var dataSourceBuilder = new Npgsql.NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.EnableDynamicJson();
            var dataSource = dataSourceBuilder.Build();

            services.AddDbContext<NutritionDbContext>(options =>
            {
                options.UseNpgsql(dataSource, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(NutritionDbContext).Assembly.FullName);
                });
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IMealLogRepository, MealLogRepository>();
            services.AddScoped<IDailyTargetRepository, DailyTargetRepository>();
            services.AddScoped<IMealPlanTemplateRepository, MealPlanTemplateRepository>();
            services.AddScoped<IWeightLogRepository, WeightLogRepository>();
            services.AddScoped<IUserTemplateQueueRepository, UserTemplateQueueRepository>();

            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddHttpContextAccessor();

            return services;
        }
    }
}
