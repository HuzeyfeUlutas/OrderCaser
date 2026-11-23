using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderCase.Processing.Application.Abstractions.Messaging;
using OrderCase.Processing.Application.Abstractions.Payments;
using OrderCase.Processing.Application.Abstractions.Persistence;
using OrderCase.Processing.Application.Abstractions.Services;
using OrderCase.Processing.Infrastructure.Correlation;
using OrderCase.Processing.Infrastructure.Messaging;
using OrderCase.Processing.Infrastructure.Payments;
using OrderCase.Processing.Infrastructure.Transactions;
using OrderCase.Processing.Persistence;

namespace OrderCase.Processing.Infrastructure;

public static class DependencyInjection
{
    public const string ConsumerGroup = "processing-service";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICorrelationContext, CorrelationContext>();
        services.AddSingleton<IPaymentGateway, FakePaymentGateway>();

        services.AddScoped<ITransactionManager, CapTransactionManager>();
        services.AddScoped<IEventPublisher, CapEventPublisher>();

        services.AddCap(options =>
        {
            options.UseEntityFramework<ProcessingDbContext>();

            options.UseRabbitMQ(rabbit =>
            {
                rabbit.HostName = configuration["RabbitMq:Host"] ?? "localhost";
                rabbit.Port = int.Parse(configuration["RabbitMq:Port"] ?? "5672");
                rabbit.UserName = configuration["RabbitMq:Username"] ?? "guest";
                rabbit.Password = configuration["RabbitMq:Password"] ?? "guest";
            });

            options.DefaultGroupName = ConsumerGroup;
            options.FailedRetryCount = 5;
            options.FailedRetryInterval = 10;
        });

        return services;
    }
}