using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OrderCase.Processing.Persistence;

public sealed class ProcessingDbContextFactory : IDesignTimeDbContextFactory<ProcessingDbContext>
{
    public ProcessingDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ProcessingDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=processingdb;Username=postgres;Password=postgres")
            .Options;

        return new ProcessingDbContext(options);
    }
}