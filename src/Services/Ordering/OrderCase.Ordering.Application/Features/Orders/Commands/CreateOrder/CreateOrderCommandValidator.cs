using FluentValidation;

namespace OrderCase.Ordering.Application.Features.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Quantity)
            .GreaterThan(0);

        RuleFor(x => x.PaymentMethod)
            .IsInEnum();
    }
}