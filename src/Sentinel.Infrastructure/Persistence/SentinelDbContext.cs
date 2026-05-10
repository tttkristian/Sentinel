using Microsoft.EntityFrameworkCore;
using Sentinel.Domain.Entities;

namespace Sentinel.Infrastructure.Persistence;


public sealed class SentinelDbContext(DbContextOptions<SentinelDbContext> options) : DbContext(options)
{
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Business> Businesses => Set<Business>();
    public DbSet<AfterHours> AfterHours => Set<AfterHours>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerCall> CustomerCalls => Set<CustomerCall>();
    public DbSet<Operator> Operators => Set<Operator>();
    public DbSet<Call> Calls => Set<Call>();
    public DbSet<Transcription> Transcriptions => Set<Transcription>();
    public DbSet<TranscriptSegment> TranscriptSegments => Set<TranscriptSegment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SentinelDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

}
