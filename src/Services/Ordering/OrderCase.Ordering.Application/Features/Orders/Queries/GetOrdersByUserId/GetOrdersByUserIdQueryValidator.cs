using FluentValidation;

namespace OrderCase.Ordering.Application.Features.Orders.Queries.GetOrdersByUserId;

public sealed class GetOrdersByUserIdQueryValidator : AbstractValidator<GetOrdersByUserIdQuery>
{
    public GetOrdersByUserIdQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .MaximumLength(100);
    }
}