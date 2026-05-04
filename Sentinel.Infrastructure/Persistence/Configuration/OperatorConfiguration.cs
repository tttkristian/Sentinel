using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentinel.Domain.Entities;

namespace Sentinel.Infrastructure.Persistence.Configuration;


public sealed class OperatorConfiguration : IEntityTypeConfiguration<Operator>
{
    public void Configure(EntityTypeBuilder<Operator> builder)
    {
        builder.ToTable("Operators", schema: "sentinel");
        builder.HasKey(o => o.OperatorId);
        builder.Property(o => o.OperatorId).ValueGeneratedNever();

        builder.Property(o => o.OperatorName).IsRequired().HasMaxLength(200);
        builder.Property(o => o.Email).IsRequired().HasMaxLength(200);
        builder.Property(o => o.RealPhoneNumber).IsRequired().HasMaxLength(20);
        builder.Property(o => o.Priority).IsRequired();
        builder.Property(o => o.IsAvailable).IsRequired();
        builder.Property(o => o.IsActive).IsRequired();
        builder.Property(o => o.IsDeleted).IsRequired();
        builder.Property(o => o.IsDeleted)
           .HasDefaultValue(false);


        builder.Property(o => o.CreatedAt)
            .IsRequired();


        builder.Property(o => o.UpdatedAt)
            .IsRequired();
            

        builder.HasMany(c => c.Calls)
            .WithOne(o => o.Operator)
            .HasForeignKey<Call>(c => c.OperatorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(o => o.Email).IsUnique();
        builder.HasIndex(o => o.IsDeleted);

        builder.HasQueryFilter(o => !o.IsDeleted);
    }
}
