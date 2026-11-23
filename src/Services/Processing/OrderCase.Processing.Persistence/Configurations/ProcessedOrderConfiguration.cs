using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderCase.Processing.Domain.ProcessedOrders;

namespace OrderCase.Processing.Persistence.Configurations;

public sealed class ProcessedOrderConfiguration : IEntityTypeConfiguration<ProcessedOrder>
{
    public void Configure(EntityTypeBuilder<ProcessedOrder> builder)
    {
        builder.ToTable("ProcessedOrders");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.OrderId).IsRequired();
        builder.Property(p => p.UserId).HasMaxLength(100).IsRequired();
        builder.Property(p => p.ProductId).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Quantity).IsRequired();
        builder.Property(p => p.PaymentMethod).HasMaxLength(20).IsRequired();

        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(p => p.FailureReason).HasMaxLength(500);

        builder.Property(p => p.CreatedBy).HasMaxLength(100);
        builder.Property(p => p.UpdatedBy).HasMaxLength(100);
        builder.Property(p => p.DeletedBy).HasMaxLength(100);

        builder.HasIndex(p => p.OrderId).IsUnique();
    }
}