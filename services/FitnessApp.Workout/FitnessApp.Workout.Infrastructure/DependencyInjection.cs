using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Application.Interfaces;
using FitnessApp.Workout.Domain.Interfaces.Persistence;
using FitnessApp.Workout.Infrastructure.Persistence;
using FitnessApp.Workout.Infrastructure.Repositories;
using FitnessApp.Workout.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitnessApp.Workout.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
           
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not set");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                });
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ITrainingProgramRepository, TrainingProgramRepository>();
            services.AddScoped<IWorkoutSessionRepository, WorkoutSessionRepository>();

          
            services.AddScoped<ICurrentUserService, CurrentUserService>();

           
            services.AddHttpContextAccessor();

            return services;
        }
    }
}
