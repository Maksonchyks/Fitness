using FitnessApp.Identity.API.Middleware;
using FitnessApp.Identity.Infrastructure.Data.Seed;
using FitnessApp.Identity.Infrastructure.Data;
using Serilog;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Identity.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseApi(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseSerilogRequestLogging();

            if (!configuration.GetValue<bool>("DisableHttpsRedirect"))
            {
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseCors("AllowSpecificOrigins");

            app.UseRateLimiter();

            app.UseAuthentication();
            app.UseAuthorization();

            if (configuration.GetValue<bool>("EnableSwagger", true))
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "FitnessApp Identity API v1");
                    options.RoutePrefix = "swagger";
                    options.DisplayRequestDuration();
                });
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                if (configuration.GetValue<bool>("HealthChecks:Enabled", true))
                {
                    var endpoint = configuration["HealthChecks:Endpoint"] ?? "/health";
                    endpoints.MapHealthChecks(endpoint);
                }
            });

            return app;
        }

        public static async Task InitializeDatabaseAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<IdentityDbContext>();

                if (context.Database.IsRelational())
                {
                    await context.Database.MigrateAsync();

                    var seeder = services.GetRequiredService<DatabaseSeeder>();
                    await seeder.SeedAsync();
                }
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while initializing the database");
                throw;
            }
        }
    }
}
