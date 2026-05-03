using Sentinel.Domain.Interfaces;

namespace Sentinel.Domain.Entities;

public sealed class CustomerCall : IAuditable
{
    public Guid CustomerCallId { get; set; }

    public Guid CustomerId { get; set; }

    public Customer? Customer { get; set; }

    public Guid CallId { get; set; }

    public Call? Call { get; set; }

    public Guid BusinessId { get; set; }

    public Business? Business { get; set; }

    public DateTime CalledAt { get; set; }

    public int? DurationSeconds { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}