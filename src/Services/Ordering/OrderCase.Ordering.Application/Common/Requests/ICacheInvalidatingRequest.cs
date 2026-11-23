namespace OrderCase.Ordering.Application.Common.Requests;

public interface ICacheInvalidatingRequest
{
    IEnumerable<string> CacheKeysToInvalidate { get; }
}