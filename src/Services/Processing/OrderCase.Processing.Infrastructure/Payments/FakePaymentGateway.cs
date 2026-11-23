using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using OrderCase.Processing.Application.Abstractions.Payments;

namespace OrderCase.Processing.Infrastructure.Payments;

public sealed class FakePaymentGateway(ILogger<FakePaymentGateway> logger) : IPaymentGateway
{
    public const int MaxAllowedQuantity = 50;
    public const string FlakyProductId = "flaky-product";
    private const int FlakyFailureCount = 2;

    private static readonly ConcurrentDictionary<Guid, int> AttemptsByOrder = new();

    public async Task<PaymentResult> ChargeAsync(PaymentRequest request, CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(300), cancellationToken);

        if (request.ProductId == FlakyProductId)
        {
            var attempt = AttemptsByOrder.AddOrUpdate(request.OrderId, 1, (_, current) => current + 1);

            if (attempt <= FlakyFailureCount)
            {
                logger.LogWarning(
                    "Payment provider unavailable for order {OrderId} (attempt {Attempt})",
                    request.OrderId, attempt);

                throw new PaymentGatewayUnavailableException("Payment provider did not respond.");
            }
        }

        if (request.Quantity > MaxAllowedQuantity)
            return PaymentResult.Failure($"Payment declined: quantity exceeds the limit of {MaxAllowedQuantity}.");

        return PaymentResult.Success();
    }
}