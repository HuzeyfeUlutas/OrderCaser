using OrderCase.Ordering.Application.Abstractions.Caching;

namespace OrderCase.Ordering.Application.Common.Caching;

public sealed class CacheInvalidator : ICacheInvalidator
{
    private readonly HashSet<string> _keys = [];

    public void Invalidate(string key) => _keys.Add(key);

    public IReadOnlyCollection<string> PendingKeys => _keys;
}