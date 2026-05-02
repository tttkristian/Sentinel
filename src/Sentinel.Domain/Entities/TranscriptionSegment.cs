namespace Sentinel.Domain.Entities;

public sealed class TranscriptSegment
{
    public Guid SegmentId { get; set; }

    public Guid TranscriptionId { get; set; }

    public Transcription? Transcription { get; set; }

    public decimal StartSeconds { get; set; }

    public decimal EndSeconds { get; set; }

    public required string Speaker { get; set; }

    public required string Text { get; set; }
}