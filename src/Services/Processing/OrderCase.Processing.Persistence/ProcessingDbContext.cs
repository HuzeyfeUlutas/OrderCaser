using Microsoft.EntityFrameworkCore;
using OrderCase.Processing.Domain.ProcessedOrders;

namespace OrderCase.Processing.Persistence;

public sealed class ProcessingDbContext(DbContextOptions<ProcessingDbContext> options) : DbContext(options)
{
    public DbSet<ProcessedOrder> ProcessedOrders => Set<ProcessedOrder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProcessingDbContext).Assembly);
    }
}