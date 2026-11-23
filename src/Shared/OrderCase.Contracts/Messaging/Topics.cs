namespace OrderCase.Contracts.Events;

public static class Topics
{
    public const string OrderCreated = "order.created"; 
    public const string OrderCompleted = "order.completed";
    public const string OrderFailed = "order.failed";
}