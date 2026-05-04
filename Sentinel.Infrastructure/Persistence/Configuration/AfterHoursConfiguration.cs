using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentinel.Domain.Entities;

namespace Sentinel.Infrastructure.Persistence.Configuration
{
    public sealed class AfterHoursConfiguration : IEntityTypeConfiguration<AfterHours>
    {
        public void Configure(EntityTypeBuilder<AfterHours> builder)
        {
            builder.ToTable("AfterHours", schema: "sentinel");
            builder.HasKey(ah => ah.AfterHoursId);
            builder.Property(ah => ah.AfterHoursId).ValueGeneratedNever();
            builder.Property(ah => ah.StartTime)
                .IsRequired();
            builder.Property(ah => ah.EndTime).IsRequired();
            builder.Property(ah => ah.LastSwitchedAt);
            
            builder.HasIndex(a => a.BusinessId)
                .IsUnique();


        }
    }
}