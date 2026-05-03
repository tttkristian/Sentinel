using Sentinel.Domain.Interfaces;

namespace Sentinel.Domain.Entities;

public sealed class Call : IAuditable
{
    public Guid CallId { get; set; }

    public Guid BusinessId { get; set; }

    public Business? Business { get; set; }

    public Guid CustomerId { get; set; }

    public Customer? Customer { get; set; }

    public Guid? OperatorId { get; set; }

    public Operator? Operator { get; set; }

    public required string FromNumber { get; set; }

    public required string ToNumber { get; set; }

    public CallDirection Direction { get; set; } = CallDirection.Inbound;

    public CallStatus Status { get; set; } = CallStatus.Ringing;

    public RoutingDecision RoutingDecision { get; set; }

    public bool WasForwarded { get; set; }

    public bool WentToVoicemail { get; set; }

    public string? RecordingUrl { get; set; }

    public int? RecordingDuration { get; set; }

    public TranscriptionStatus TranscriptionStatus { get; set; } = TranscriptionStatus.NotApplicable;

    public DateTime StartedAt { get; set; }

    public DateTime? AnsweredAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public int? DurationSeconds { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public CustomerCall? CustomerCall { get; set; }

    public Transcription? Transcription { get; set; }
}