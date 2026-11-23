using MediatR;
using OrderCase.Ordering.Application.Abstractions.Caching;
using OrderCase.Ordering.Application.Common.Requests;

namespace OrderCase.Ordering.Application.Behaviors;

public sealed class CacheInvalidationBehavior<TRequest, TResponse>(
    ICacheService cacheService,
    ICacheInvalidator cacheInvalidator)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next(cancellationToken);

        var keys = new HashSet<string>(cacheInvalidator.PendingKeys);

        if (request is ICacheInvalidatingRequest cacheInvalidatingRequest)
            keys.UnionWith(cacheInvalidatingRequest.CacheKeysToInvalidate);

        foreach (var key in keys)
            await cacheService.RemoveAsync(key, cancellationToken);

        return response;
    }
}