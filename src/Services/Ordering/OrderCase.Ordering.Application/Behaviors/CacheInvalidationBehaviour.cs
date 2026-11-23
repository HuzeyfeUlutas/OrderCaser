using MediatR;
using OrderCase.Ordering.Application.Abstractions.Caching;
using OrderCase.Ordering.Application.Common.Requests;

namespace OrderCase.Ordering.Application.Behaviors;

public sealed class CacheInvalidationBehavior<TRequest, TResponse>(ICacheService cacheService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next(cancellationToken);

        if (request is ICacheInvalidatingRequest cacheInvalidatingRequest)
        {
            foreach (var key in cacheInvalidatingRequest.CacheKeysToInvalidate)
                await cacheService.RemoveAsync(key, cancellationToken);
        }

        return response;
    }
}