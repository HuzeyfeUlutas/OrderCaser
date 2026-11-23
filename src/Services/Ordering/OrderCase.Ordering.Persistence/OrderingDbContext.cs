using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OrderCase.Ordering.Domain.Common;
using OrderCase.Ordering.Domain.Orders;

namespace OrderCase.Ordering.Persistence;

public sealed class OrderingDbContext(DbContextOptions<OrderingDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderingDbContext).Assembly);
    }
}