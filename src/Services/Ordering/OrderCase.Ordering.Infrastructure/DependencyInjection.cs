using DotNetCore.CAP;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderCase.Ordering.Application.Abstractions.Caching;
using OrderCase.Ordering.Application.Abstractions.Messaging;
using OrderCase.Ordering.Application.Abstractions.Persistence;
using OrderCase.Ordering.Application.Abstractions.Services;
using OrderCase.Ordering.Infrastructure.Caching;
using OrderCase.Ordering.Infrastructure.Correlation;
using OrderCase.Ordering.Infrastructure.Messaging;
using OrderCase.Ordering.Infrastructure.Transactions;
using OrderCase.Ordering.Persistence;
using StackExchange.Redis;

namespace OrderCase.Ordering.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<CapOptions>? configureCap = null)
    {
        services.AddSingleton<ICorrelationContext, CorrelationContext>();

        AddRedis(services, configuration);
        AddCap(services, configuration, configureCap);

        return services;
    }

    private static void AddRedis(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Connection string 'Redis' is not configured.");

        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var options = ConfigurationOptions.Parse(connectionString);
            options.AbortOnConnectFail = false;
            return ConnectionMultiplexer.Connect(options);
        });

        services.AddSingleton<ICacheService, RedisCacheService>();
    }

    private static void AddCap(
        IServiceCollection services,
        IConfiguration configuration,
        Action<CapOptions>? configureCap)
    {
        services.AddScoped<ITransactionManager, CapTransactionManager>();
        services.AddScoped<IEventPublisher, CapEventPublisher>();

        services.AddCap(options =>
        {
            options.UseEntityFramework<OrderingDbContext>();

            options.UseRabbitMQ(rabbit =>
            {
                rabbit.HostName = configuration["RabbitMq:Host"] ?? "localhost";
                rabbit.Port = int.Parse(configuration["RabbitMq:Port"] ?? "5672");
                rabbit.UserName = configuration["RabbitMq:Username"] ?? "guest";
                rabbit.Password = configuration["RabbitMq:Password"] ?? "guest";
            });

            options.DefaultGroupName = "ordering-service";
            options.FailedRetryCount = 5;
            options.FailedRetryInterval = 10;

            configureCap?.Invoke(options);
        });
    }
}