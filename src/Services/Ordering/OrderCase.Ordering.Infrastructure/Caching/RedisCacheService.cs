using System.Text.Json;
using Microsoft.Extensions.Logging;
using OrderCase.Ordering.Application.Abstractions.Caching;
using StackExchange.Redis;

namespace OrderCase.Ordering.Infrastructure.Caching;

public sealed class RedisCacheService(
    IConnectionMultiplexer redis,
    ILogger<RedisCacheService> logger) : ICacheService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken) where T : class
    {
        try
        {
            var value = await redis.GetDatabase().StringGetAsync(key);

            if (value.IsNullOrEmpty)
            {
                logger.LogInformation("Cache miss for {CacheKey}", key);
                return null;
            }

            logger.LogInformation("Cache hit for {CacheKey}", key);
            return JsonSerializer.Deserialize<T>(value.ToString(), JsonOptions);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Cache read failed for {CacheKey}, falling back to database", key);
            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken)
        where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(value, JsonOptions);
            await redis.GetDatabase().StringSetAsync(key, json, expiration);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Cache write failed for {CacheKey}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken)
    {
        try
        {
            await redis.GetDatabase().KeyDeleteAsync(key);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Cache remove failed for {CacheKey}", key);
        }
    }
}