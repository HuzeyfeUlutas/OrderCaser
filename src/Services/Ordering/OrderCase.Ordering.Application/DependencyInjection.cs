using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OrderCase.Ordering.Application.Abstractions.Caching;
using OrderCase.Ordering.Application.Behaviors;
using OrderCase.Ordering.Application.Common.Caching;

namespace OrderCase.Ordering.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(CacheInvalidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });

        services.AddScoped<ICacheInvalidator, CacheInvalidator>();
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}