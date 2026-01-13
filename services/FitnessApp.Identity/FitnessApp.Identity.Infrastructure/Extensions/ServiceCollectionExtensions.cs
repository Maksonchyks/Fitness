using System;
using System.Collections.Generic;
using System.Linq;
using FitnessApp.Identity.Application.Interfaces;
using FitnessApp.Identity.Domain.Interfaces.Repositories;
using FitnessApp.Identity.Infrastructure.Data;
using FitnessApp.Identity.Infrastructure.Repositories;
using FitnessApp.Identity.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Identity.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext(configuration);

            services.AddRepositories();

            services.AddServices(configuration);

            services.AddCaching(configuration);

            return services;
        }

        private static IServiceCollection AddDbContext(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("IdentityDb");

            services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName);
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);
                });

                if (configuration.GetValue<bool>("EnableEFCoreLogging"))
                {
                    options.EnableSensitiveDataLogging();
                    options.LogTo(Console.WriteLine, LogLevel.Information);
                }
            });

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        private static IServiceCollection AddServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IDateTimeService, DateTimeService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // HttpContext для CurrentUserService
            services.AddHttpContextAccessor();

            return services;
        }

        private static IServiceCollection AddCaching(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var redisConnectionString = configuration.GetConnectionString("Redis");

            if (!string.IsNullOrEmpty(redisConnectionString))
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConnectionString;
                    options.InstanceName = "IdentityCache:";
                });
            }
            else
            {
                services.AddDistributedMemoryCache();
            }
            services.AddScoped<IRedisCacheService, RedisCacheService>();

            return services;
        }
    }
}
