namespace OrderCase.Ordering.Application.Abstractions.Caching;

public interface ICacheInvalidator
{
    void Invalidate(string key);
    IReadOnlyCollection<string> PendingKeys { get; }
}