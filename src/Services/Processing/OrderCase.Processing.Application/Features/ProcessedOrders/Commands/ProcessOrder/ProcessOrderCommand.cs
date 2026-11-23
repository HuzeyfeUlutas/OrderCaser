using MediatR;
using OrderCase.Processing.Application.Common.Requests;

namespace OrderCase.Processing.Application.Features.ProcessedOrders.Commands.ProcessOrder;

public sealed record ProcessOrderCommand(
    Guid OrderId,
    string UserId,
    string ProductId,
    int Quantity,
    string PaymentMethod) : IRequest, ITransactionalRequest;