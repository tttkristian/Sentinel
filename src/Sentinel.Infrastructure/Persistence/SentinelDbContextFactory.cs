using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Sentinel.Infrastructure.Persistence;

public class SentinelDbContextFactory : IDesignTimeDbContextFactory<SentinelDbContext>
{
    public SentinelDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Sentinel.Api"))
            .AddJsonFile("appsettings.json")
            .AddUserSecrets("a41926a7-8a2d-42fe-bb77-a9fe7eb8792c")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<SentinelDbContext>();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

        return new SentinelDbContext(optionsBuilder.Options);
    }
}
