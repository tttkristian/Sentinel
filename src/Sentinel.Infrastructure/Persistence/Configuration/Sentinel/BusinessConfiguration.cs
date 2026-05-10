using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentinel.Domain.Entities;

namespace Sentinel.Infrastructure.Persistence.Configuration.Sentinel
{
    public sealed class BusinessConfiguration : IEntityTypeConfiguration<Business>
    {
        public void Configure(EntityTypeBuilder<Business> builder)
        {
            
            builder.HasKey(b => b.BusinessId);
            builder.Property(b => b.BusinessId).ValueGeneratedNever();

            builder.Property(b => b.BusinessName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b => b.OwnerRealPhone)
                .IsRequired().HasMaxLength(100);
            builder.Property(b => b.VirtualBusinessPhone).IsRequired().HasMaxLength(200);
            builder.Property(b => b.RecordCalls).IsRequired();
            builder.Property(b => b.IsActive).IsRequired();
            builder.Property(b => b.IsDeleted).IsRequired();
           
            builder.HasOne(b => b.AfterHours)
                .WithOne(ah => ah.Business)
                .HasForeignKey<AfterHours>(ah => ah.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(b => b.VirtualBusinessPhone).IsUnique();
            builder.HasIndex(b => b.IsDeleted);
            builder.HasIndex(b => b.OwnerRealPhone).IsUnique();

            builder.HasQueryFilter(b => !b.IsDeleted);
        }
    }
}