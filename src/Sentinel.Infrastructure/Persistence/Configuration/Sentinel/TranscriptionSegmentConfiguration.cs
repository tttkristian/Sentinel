
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentinel.Domain.Entities;

namespace Sentinel.Infrastructure.Persistence.Configuration.Sentinel;

// TranscriptSegmentConfiguration.cs
public sealed class TranscriptSegmentConfiguration : IEntityTypeConfiguration<TranscriptSegment>
{
    public void Configure(EntityTypeBuilder<TranscriptSegment> builder)
    {
        
        builder.HasKey(s => s.SegmentId);
        builder.Property(s => s.SegmentId).ValueGeneratedNever();

        builder.Property(s => s.StartSeconds).HasColumnType("decimal(10,3)");
        builder.Property(s => s.EndSeconds).HasColumnType("decimal(10,3)");
        builder.Property(s => s.Speaker).IsRequired().HasMaxLength(50);
        builder.Property(s => s.Text).IsRequired().HasMaxLength(2000);

        builder.HasOne(s => s.Transcription)
            .WithMany(t => t.Segments)
            .HasForeignKey(s => s.TranscriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => s.TranscriptionId);
    }
}
