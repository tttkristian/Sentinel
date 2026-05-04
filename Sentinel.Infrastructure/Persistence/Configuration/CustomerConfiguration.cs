// CustomerConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentinel.Domain.Entities;

namespace Sentinel.Infrastructure.Persistence.Configuration;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {

        builder.ToTable("Customers", schema: "sentinel");
        builder.HasKey(c => c.CustomerId);
        builder.Property(c => c.CustomerId).ValueGeneratedNever();

        builder.Property(c => c.PhoneNumber).IsRequired().HasMaxLength(20);
        builder.Property(c => c.CustomerName).HasMaxLength(200);
        builder.Property(c => c.Email).HasMaxLength(200);
        builder.Property(c => c.IsDeleted).IsRequired();
        builder.Property(c => c.LastSeenAt).IsRequired();

        builder.HasMany(c => c.CustomerCalls)
            .WithOne(cc => cc.Customer)
            .HasForeignKey(cc => cc.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.PhoneNumber).IsUnique();
        builder.HasIndex(c => c.Email).IsUnique();
        builder.HasIndex(c => c.IsDeleted);

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
