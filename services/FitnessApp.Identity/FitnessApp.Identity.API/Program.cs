using FitnessApp.Identity.API.Extensions;
using FitnessApp.Identity.Application.Extensions;
using FitnessApp.Identity.Infrastructure.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddSerilogLogging(builder.Configuration);
builder.Services.AddApi(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseApi(builder.Configuration);

if (builder.Configuration.GetValue<bool>("InitializeDatabase", true))
{
    await app.InitializeDatabaseAsync();
}


app.Run();

public partial class Program { }