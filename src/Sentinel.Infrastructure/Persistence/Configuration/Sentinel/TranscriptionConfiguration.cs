using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentinel.Domain.Entities;

namespace Sentinel.Infrastructure.Persistence.Configuration.Sentinel;

// TranscriptionConfiguration.cs
public sealed class TranscriptionConfiguration : IEntityTypeConfiguration<Transcription>
{
    public void Configure(EntityTypeBuilder<Transcription> builder)
    {
        
        builder.HasKey(t => t.TranscriptionId);
        builder.Property(t => t.TranscriptionId).ValueGeneratedNever();

        builder.Property(t => t.FullText).IsRequired();  
        builder.Property(t => t.Language).IsRequired().HasMaxLength(10);
        builder.Property(t => t.Provider).IsRequired().HasMaxLength(50);
        builder.Property(t => t.ConfidenceScore).HasColumnType("decimal(5,4)");
        builder.Property(t => t.RawJson);                
        builder.Property(t => t.DurationProcessed).IsRequired();

        // Call ↔ Transcription 1:1 already declared on Call side.

        builder.HasIndex(t => t.CallId).IsUnique();
    }
}
