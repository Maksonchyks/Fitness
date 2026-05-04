using FitnessApp.Communication.Infrastructure.Persistence;
using FitnessApp.Communication.Infrastructure.Persistence.Repositories;
using FitnessApp.Communication.Infrastructure.Hubs;
using FitnessApp.Communication.Infrastructure.Messaging.Consumers;
using FitnessApp.Communication.Infrastructure.BackgroundServices;
using FitnessApp.Communication.Infrastructure.Services;
using FitnessApp.Communication.Application.Interfaces;
using FitnessApp.Communication.Application.Features.Chats.Commands.SendMessage;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog;
using FluentValidation;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();

// Add services to the container
// Authentication & Authorization
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secret = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret missing");
var key = Encoding.UTF8.GetBytes(secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        ClockSkew = TimeSpan.Zero,
        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
        RoleClaimType = "http://schemas.microsoft.com/w32/2008/06/identity/claims/role"
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/communicationHub"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "FitnessApp Communication API", Version = "v1" });
});

// Database
builder.Services.AddDbContext<CommunicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection - Repositories
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Dependency Injection - Services
builder.Services.AddScoped<INotificationService, SignalRNotificationService>();

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SendMessageCommand).Assembly));

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(SendMessageCommand).Assembly);

// SignalR
builder.Services.AddSignalR();

// MassTransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserCreatedConsumer>();
    x.AddConsumer<UserRoleChangedConsumer>();
    x.AddConsumer<UserStatusChangedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });

        cfg.ReceiveEndpoint("communication-user-events", e =>
        {
            e.ConfigureConsumer<UserCreatedConsumer>(context);
            e.ConfigureConsumer<UserRoleChangedConsumer>(context);
            e.ConfigureConsumer<UserStatusChangedConsumer>(context);
        });
    });
});

// Background Service
builder.Services.AddHostedService<ReminderBackgroundService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
        .WithOrigins("http://localhost:3000", "http://localhost:5173", "http://localhost:5174")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<CommunicationHub>("/communicationHub");

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CommunicationDbContext>();
    db.Database.Migrate();
}

// Reset all users to offline on startup
using (var scope = app.Services.CreateScope())
{
    var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
    var users = await userRepository.GetAllAsync(default);
    foreach (var user in users)
    {
        user.SetStatus(false);
    }
    await userRepository.SaveChangesAsync(default);
}

app.Run();
