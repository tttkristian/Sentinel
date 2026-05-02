using Sentinel.Domain.Interfaces;

namespace CallCenter.Domain.Entities;

public sealed class Customer : ISoftDelete
{
    public Guid CustomerId { get; set; }

    public required string PhoneNumber { get; set; }

    public string? CustomerName { get; set; }
    public string? Email { get; set; }
    public CustomerCalls? CustomerCalls { get; set; } 
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public DateTime FirstSeenAt { get; set; }

    public DateTime LastSeenAt { get; set; }

    public ICollection<Call> Calls { get; set; } = [];
}