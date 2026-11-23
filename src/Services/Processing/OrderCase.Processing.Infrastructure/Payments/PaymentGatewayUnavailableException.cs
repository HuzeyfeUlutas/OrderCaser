namespace OrderCase.Processing.Infrastructure.Payments;

public sealed class PaymentGatewayUnavailableException(string message) : Exception(message);