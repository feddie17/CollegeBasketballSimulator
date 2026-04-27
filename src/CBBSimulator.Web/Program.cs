using CBBSimulator.Core.Configuration;
using CBBSimulator.Data.Caching;
using CBBSimulator.Data.Scrapers;
using CBBSimulator.Data.Services;
using CBBSimulator.Web.BackgroundServices;
using CBBSimulator.Web.Endpoints;
using CBBSimulator.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Services.AddSingleton(new SimulationConfig());

// Data layer
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICsvDataLoader, CsvDataLoader>();
builder.Services.AddSingleton<TeamDataCache>();
builder.Services.AddSingleton<ITeamDataService, TeamDataService>();

// Background services
builder.Services.AddHostedService<DataRefreshService>();
builder.Services.AddSingleton<SimulationQueue>();
builder.Services.AddHostedService<SimulationWorkerService>();

// SignalR
builder.Services.AddSignalR();

// CORS (for Vue dev server)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Middleware
app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

// REST endpoints
app.MapTeamsEndpoints();
app.MapSimulationEndpoints();
app.MapHealthEndpoints();

// SignalR hubs
app.MapHub<GameHub>("/hubs/game");
app.MapHub<TournamentHub>("/hubs/tournament");
app.MapHub<SeasonHub>("/hubs/season");

// SPA fallback — serve index.html for unmatched routes
app.MapFallbackToFile("index.html");

app.Run();

// Make Program accessible for integration tests
public partial class Program { }
