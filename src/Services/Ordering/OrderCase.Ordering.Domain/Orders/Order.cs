using OrderCase.Ordering.Domain.Common;

namespace OrderCase.Ordering.Domain.Orders;

public sealed class Order : AuditableEntity
{
    public string UserId { get; private set; } = null!;
    public string ProductId { get; private set; } = null!;
    public int Quantity { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public OrderStatus Status { get; private set; }
    public string? FailureReason { get; private set; }
    
    private Order() { }

    public static Order Create(string userId, string productId, int quantity, PaymentMethod paymentMethod)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new DomainException("UserId is required.");

        if (string.IsNullOrWhiteSpace(productId))
            throw new DomainException("ProductId is required.");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        return new Order
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            ProductId = productId,
            Quantity = quantity,
            PaymentMethod = paymentMethod,
            Status = OrderStatus.Pending
        };
    }

    public void Complete()
    {
        if (Status == OrderStatus.Completed)
            return; 

        if (Status != OrderStatus.Pending)
            throw new DomainException($"Order in '{Status}' status cannot be completed.");

        Status = OrderStatus.Completed;
    }

    public void Fail(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Failure reason is required.");

        if (Status == OrderStatus.Failed)
            return; 

        if (Status != OrderStatus.Pending)
            throw new DomainException($"Order in '{Status}' status cannot be failed.");

        Status = OrderStatus.Failed;
        FailureReason = reason;
    }
} 