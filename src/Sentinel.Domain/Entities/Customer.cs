using Sentinel.Domain.Interfaces;

namespace Sentinel.Domain.Entities;

public sealed class Customer : ISoftDelete, IAuditable
{
    public Guid CustomerId { get; set; }

    public required string PhoneNumber { get; set; }

    public string? CustomerName { get; set; }
    public string? Email { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime LastSeenAt { get; set;  }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<Call> Calls { get; set; } = [];
    public ICollection<CustomerCalls> CustomerCalls { get; set; } = [];
}