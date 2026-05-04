using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentinel.Domain.Entities;

namespace Sentinel.Infrastructure.Persistence.Configuration
{
    public sealed class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.ToTable("Admins", schema: "sentinel");
            builder.HasKey(a => a.AdminId);
            builder.Property(a => a.AdminId).ValueGeneratedNever();

            builder.Property(a => a.AdminFullName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(a => a.Email)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(a => a.Permissions)
                .HasMaxLength(10);
            builder.Property(a => a.IsActive)
                .IsRequired();

            builder.HasIndex(a => a.Email)
                .IsUnique();
            builder.Property(a => a.AdminPhoneNumber)
                .IsRequired();
        }
    }
}