namespace OrderCase.Ordering.Application.Abstractions.Caching;

public static class OrderCacheKeys
{
    public static string ById(Guid orderId) => $"order:{orderId}";
}