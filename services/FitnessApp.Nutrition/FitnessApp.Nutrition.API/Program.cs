using FitnessApp.Nutrition.Application;
using FitnessApp.Nutrition.Infrastructure;
using FitnessApp.Nutrition.API.Extensions;
using FitnessApp.Nutrition.Infrastructure.Persistence;
using FitnessApp.Nutrition.Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApiAuthentication(builder.Configuration);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", builder =>
        builder.WithOrigins("http://localhost:3000", "http://localhost:5173", "http://localhost:5174")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

// Apply migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NutritionDbContext>();
    await db.Database.MigrateAsync();

    // Seed meal plan templates if empty
    if (!await db.MealPlanTemplates.AnyAsync())
    {
        var templates = MealPlanTemplateSeed.GetTemplates();
        await db.MealPlanTemplates.AddRangeAsync(templates);
        await db.SaveChangesAsync();
    }
}

app.MapControllers();

app.Run();
