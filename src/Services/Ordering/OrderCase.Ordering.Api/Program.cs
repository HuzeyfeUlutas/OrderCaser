using System.Text.Json.Serialization;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using OrderCase.Ordering.Api.Authentication;
using OrderCase.Ordering.Api.ExceptionHandling;
using OrderCase.Ordering.Api.Middlewares;
using OrderCase.Ordering.Api.Services;
using OrderCase.Ordering.Api.Subscribers;
using OrderCase.Ordering.Application;
using OrderCase.Ordering.Application.Abstractions.Services;
using OrderCase.Ordering.Infrastructure;
using OrderCase.Ordering.Persistence;
using OrderCase.Ordering.Persistence.Abstractions;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// --- Logging ---
builder.Host.UseSerilog((_, _, logger) => logger
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "Ordering.Api")
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] [{Service}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}"));

// --- Web ---
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Ordering API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Description = "Test tokens: admin-token-123, user-token-123, user-token-456"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// --- Authentication & current user ---
builder.Services.Configure<AuthenticationSettings>(
    builder.Configuration.GetSection(AuthenticationSettings.SectionName));

builder.Services
    .AddAuthentication(TokenAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, TokenAuthenticationHandler>(
        TokenAuthenticationHandler.SchemeName, _ => { });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddScoped<ICurrentUserService>(sp => sp.GetRequiredService<CurrentUserService>());
builder.Services.AddScoped<IAuditUserProvider>(sp => sp.GetRequiredService<CurrentUserService>());

// --- Subscribers ---
builder.Services.AddTransient<OrderStatusSubscriber>();

// --- Layers ---
var connectionString = builder.Configuration.GetConnectionString("OrderingDb")
    ?? throw new InvalidOperationException("Connection string 'OrderingDb' is not configured.");

builder.Services.AddPersistence(connectionString);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, cap =>
    cap.UseDashboard(dashboard => dashboard.AllowAnonymousExplicit = true));

var app = builder.Build();

// --- Database ---
await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();
    await dbContext.Database.MigrateAsync();
}

// --- Pipeline ---
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging();
app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();