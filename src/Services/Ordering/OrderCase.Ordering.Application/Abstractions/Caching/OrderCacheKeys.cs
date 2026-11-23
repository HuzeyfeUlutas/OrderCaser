namespace OrderCase.Ordering.Application.Abstractions.Caching;

public static class OrderCacheKeys
{
    public static string ById(Guid orderId) => $"order:{orderId}";
    public static string ByUser(string userId) => $"orders:user:{userId}";
}