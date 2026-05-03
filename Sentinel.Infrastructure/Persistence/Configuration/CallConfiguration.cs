using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentinel.Domain.Entities;

namespace Sentinel.Infrastructure.Persistence.Configuration
{
    public sealed class CallConfiguration : IEntityTypeConfiguration<Call>
    {
        public void Configure(EntityTypeBuilder<Call> builder)
        {
            builder.ToTable("Calls", schema: "sentinel");
            builder.HasKey(c => c.CallId);

            builder.Property(c => c.CallId).ValueGeneratedNever();

            builder.Property(c => c.FromNumber)
                .IsRequired()
                .HasMaxLength(20);
            builder.Property(c => c.ToNumber)
                .IsRequired()
                .HasMaxLength(20);
            builder.Property(c => c.RecordingUrl)
                .HasMaxLength(500);
            builder.Property(c => c.StartedAt)
                .IsRequired();

            builder.Property(c => c.OperatorId)
                .IsRequired(false);
            builder.Property(c => c.WentToVoicemail)
                .IsRequired();
            builder.Property(c => c.DurationSeconds);

            builder.HasOne(c => c.Operator)
                .WithMany(o => o.Calls)
                .HasForeignKey(c => c.OperatorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Business)
                .WithMany(b => b.Calls)
                .HasForeignKey(c => c.BusinessId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasOne(c => c.Customer)
                .WithMany(cu => cu.Calls)
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
           
            builder.HasOne(c => c.Operator)
                .WithMany(o => o.Calls)
                .HasForeignKey(c => c.OperatorId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasOne(c => c.CustomerCall)
                .WithOne(cc => cc.Call)
                .HasForeignKey<CustomerCall>(cc => cc.CallId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(c => c.Transcription)
                .WithOne(t => t.Call)
                .HasForeignKey<Transcription>(t => t.CallId) .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(c => c.BusinessId);
            builder.HasIndex(c => c.CustomerId); 
            builder.HasIndex(c => c.StartedAt);
            

        }
    }
}
