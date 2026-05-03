using Sentinel.Domain.Interfaces;

namespace Sentinel.Domain.Entities;

public sealed class AfterHours : IAuditable
{
    public Guid AfterHoursId { get; set; }

    public Guid BusinessId { get; set; }

    public Business? Business { get; set; }

    public bool IsAfterHours { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public DateTime? LastSwitchedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}