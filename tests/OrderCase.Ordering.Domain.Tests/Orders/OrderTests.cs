using FluentAssertions;
using OrderCase.Ordering.Domain.Common;
using OrderCase.Ordering.Domain.Orders;

namespace OrderCase.Ordering.Domain.Tests.Orders;

public class OrderTests
{
    private static Order CreateValidOrder() =>
        Order.Create("user-1", "product-1", 2, PaymentMethod.CreditCard);

    [Fact]
    public void Create_WithValidData_ReturnsPendingOrder()
    {
        var order = CreateValidOrder();

        order.Id.Should().NotBeEmpty();
        order.UserId.Should().Be("user-1");
        order.ProductId.Should().Be("product-1");
        order.Quantity.Should().Be(2);
        order.PaymentMethod.Should().Be(PaymentMethod.CreditCard);
        order.Status.Should().Be(OrderStatus.Pending);
        order.IsDeleted.Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithNonPositiveQuantity_ThrowsDomainException(int quantity)
    {
        var act = () => Order.Create("user-1", "product-1", quantity, PaymentMethod.CreditCard);

        act.Should().Throw<DomainException>().WithMessage("*Quantity*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyUserId_ThrowsDomainException(string userId)
    {
        var act = () => Order.Create(userId, "product-1", 1, PaymentMethod.CreditCard);

        act.Should().Throw<DomainException>().WithMessage("*UserId*");
    }

    [Fact]
    public void Complete_WhenPending_SetsStatusToCompleted()
    {
        var order = CreateValidOrder();

        order.Complete();

        order.Status.Should().Be(OrderStatus.Completed);
    }

    [Fact]
    public void Complete_WhenAlreadyCompleted_IsIdempotent()
    {
        var order = CreateValidOrder();
        order.Complete();

        var act = () => order.Complete();

        act.Should().NotThrow();
        order.Status.Should().Be(OrderStatus.Completed);
    }

    [Fact]
    public void Complete_WhenFailed_ThrowsDomainException()
    {
        var order = CreateValidOrder();
        order.Fail("Payment rejected");

        var act = () => order.Complete();

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Fail_WhenPending_SetsStatusAndReason()
    {
        var order = CreateValidOrder();

        order.Fail("Payment rejected");

        order.Status.Should().Be(OrderStatus.Failed);
        order.FailureReason.Should().Be("Payment rejected");
    }

    [Fact]
    public void Fail_WithEmptyReason_ThrowsDomainException()
    {
        var order = CreateValidOrder();

        var act = () => order.Fail("");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Fail_WhenCompleted_ThrowsDomainException()
    {
        var order = CreateValidOrder();
        order.Complete();

        var act = () => order.Fail("Too late");

        act.Should().Throw<DomainException>();
    }
}