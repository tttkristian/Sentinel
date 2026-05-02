using Sentinel.Domain.Interfaces;

namespace Sentinel.Domain.Entities;

public sealed class Operator : ISoftDelete, IActivatable, IAuditable
{
    public Guid OperatorId { get; set; }

    public required string OperatorName { get; set; }

    public required string Email { get; set; }

    public required string RealPhoneNumber { get; set; }

    public bool Priority { get; set; } // when this is true, the operator will be called first instead of the owner.

    public bool IsAvailable { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<Call> Calls { get; set; } = [];
}