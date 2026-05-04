using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentinel.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sentinel.Infrastructure.Persistence.Configuration;


public sealed class CustomerCallConfiguration : IEntityTypeConfiguration<CustomerCall>
{
    public void Configure(EntityTypeBuilder<CustomerCall> builder)
    {
        builder.ToTable("CustomerCalls", schema: "sentinel");
        builder.HasKey(cc => cc.CustomerCallId);
        builder.Property(cc => cc.CustomerCallId).ValueGeneratedNever();

        builder.Property(cc => cc.CalledAt).IsRequired();
        builder.Property(cc => cc.DurationSeconds);
        builder.Property(cc => cc.Notes).HasMaxLength(1000);
        builder.Property(cc => cc.CallId).IsRequired();


        builder.HasOne(cc => cc.Customer)
            .WithMany(c => c.CustomerCalls)   
            .HasForeignKey(cc => cc.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cc => cc.Business)
            .WithMany()
            .HasForeignKey(cc => cc.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cc => cc.Call)
            .WithMany()
            .HasForeignKey(cc => cc.CallId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(cc => cc.CustomerId);
        builder.HasIndex(cc => cc.BusinessId);
        builder.HasIndex(cc => cc.CalledAt);
        builder.HasIndex(cc => cc.CallId);
    }
}