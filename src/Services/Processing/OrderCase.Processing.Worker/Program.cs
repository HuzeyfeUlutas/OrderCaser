using Microsoft.EntityFrameworkCore;
using OrderCase.Processing.Application;
using OrderCase.Processing.Infrastructure;
using OrderCase.Processing.Persistence;
using OrderCase.Processing.Persistence.Abstractions;
using OrderCase.Processing.Worker.Services;
using OrderCase.Processing.Worker.Subscribers;
using Serilog;
using Serilog.Events;

var builder = Host.CreateApplicationBuilder(args);

// --- Logging ---
builder.Services.AddSerilog(logger => logger
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "Processing.Worker")
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] [{Service}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}"));

// --- Layers ---
var connectionString = builder.Configuration.GetConnectionString("ProcessingDb")
                       ?? throw new InvalidOperationException("Connection string 'ProcessingDb' is not configured.");

builder.Services.AddSingleton<IAuditUserProvider, SystemAuditUserProvider>();

builder.Services.AddPersistence(connectionString);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// --- Subscribers ---
builder.Services.AddTransient<OrderCreatedSubscriber>();

var host = builder.Build();

// --- Database ---
await using (var scope = host.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ProcessingDbContext>();
    await dbContext.Database.MigrateAsync();
}

await host.RunAsync();