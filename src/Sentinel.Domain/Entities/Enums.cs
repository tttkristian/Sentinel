namespace Sentinel.Domain.Entities;

public enum CallDirection
{
    Inbound = 0,
    OutBound = 1,
}

public enum CallStatus
{
    Ringing = 0,
    InProgress = 1,
    Completed = 2,
    NoAnswer = 3,
    Busy = 4,
    Failed = 5,
    Voicemail = 6,
}

public enum RoutingDecision
{
    None = 0,
    AnsweredByOperator = 1,
    ForwardedToOwner = 2,
    SentToVoicemail = 3,
    Rejected = 4,

}

public enum TranscriptionStatus
{
    NotApplicable = 0,
    Pending = 1,
    InProgress = 2,
    Completed = 3,
    Failed = 4,
}