using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderCase.Ordering.Domain.Orders;

namespace OrderCase.Ordering.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedNever();

        builder.Property(o => o.UserId).HasMaxLength(100).IsRequired();
        builder.Property(o => o.ProductId).HasMaxLength(100).IsRequired();
        builder.Property(o => o.Quantity).IsRequired();

        builder.Property(o => o.PaymentMethod).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(o => o.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(o => o.FailureReason).HasMaxLength(500);

        builder.Property(o => o.CreatedBy).HasMaxLength(100);
        builder.Property(o => o.UpdatedBy).HasMaxLength(100);
        builder.Property(o => o.DeletedBy).HasMaxLength(100);

        builder.HasIndex(o => o.UserId);
    }
}