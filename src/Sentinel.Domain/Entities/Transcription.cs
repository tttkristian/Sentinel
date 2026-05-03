using Sentinel.Domain.Interfaces;

namespace Sentinel.Domain.Entities;

public sealed class Transcription : IAuditable
{
    public Guid TranscriptionId { get; set; }

    public Guid CallId { get; set; }

    public Call? Call { get; set; }

    public required string FullText { get; set; }

    public required string Language { get; set; }

    public required string Provider { get; set; }

    public decimal? ConfidenceScore { get; set; }

    public string? RawJson { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public int DurationProcessed { get; set; }

    public ICollection<TranscriptSegment> Segments { get; set; } = [];
}