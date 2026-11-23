using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OrderCase.Ordering.Persistence;

public sealed class OrderingDbContextFactory : IDesignTimeDbContextFactory<OrderingDbContext>
{
    public OrderingDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<OrderingDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=orderdb;Username=postgres;Password=postgres")
            .Options;

        return new OrderingDbContext(options);
    }
}