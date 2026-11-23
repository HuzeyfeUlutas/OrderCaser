using OrderCase.Processing.Domain.Common;

namespace OrderCase.Processing.Domain.ProcessedOrders;

public sealed class ProcessedOrder : AuditableEntity
{
    public Guid OrderId { get; private set; }
    public string UserId { get; private set; } = null!;
    public string ProductId { get; private set; } = null!;
    public int Quantity { get; private set; }
    public string PaymentMethod { get; private set; } = null!;
    public ProcessingStatus Status { get; private set; }
    public string? FailureReason { get; private set; }
    
    private ProcessedOrder() { }

    public static ProcessedOrder Succeeded(
        Guid orderId, string userId, string productId, int quantity, string paymentMethod) =>
        Create(orderId, userId, productId, quantity, paymentMethod, ProcessingStatus.Succeeded, null);

    public static ProcessedOrder Failed(
        Guid orderId, string userId, string productId, int quantity, string paymentMethod, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Failure reason is required.");

        return Create(orderId, userId, productId, quantity, paymentMethod, ProcessingStatus.Failed, reason);
    }

    private static ProcessedOrder Create(
        Guid orderId, string userId, string productId, int quantity, string paymentMethod,
        ProcessingStatus status, string? failureReason)
    {
        if (orderId == Guid.Empty)
            throw new DomainException("OrderId is required.");

        return new ProcessedOrder
        {
            Id = Guid.CreateVersion7(),
            OrderId = orderId,
            UserId = userId,
            ProductId = productId,
            Quantity = quantity,
            PaymentMethod = paymentMethod,
            Status = status,
            FailureReason = failureReason
        };
    }
}