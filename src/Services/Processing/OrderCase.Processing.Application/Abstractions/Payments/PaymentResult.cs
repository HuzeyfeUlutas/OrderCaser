namespace OrderCase.Processing.Application.Abstractions.Payments;

public sealed record PaymentResult(bool IsSuccess, string? FailureReason)
{
    public static PaymentResult Success() => new(true, null);

    public static PaymentResult Failure(string reason) => new(false, reason);
}