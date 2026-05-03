using Sentinel.Domain.Interfaces;

namespace Sentinel.Domain.Entities;

    

public sealed class Business : IActivatable, IAuditable, ISoftDelete
{
    public Guid BusinessId { get; set; }
    public required string BusinessName { get; set; }
    public string OwnerRealPhone { get; set; } = null!;
    public string VirtualBusinessPhone { get; set; } = null!;  
    public bool RecordCalls { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public AfterHours? AfterHours { get; set; }
    public ICollection<Call> Calls { get; set; } = [];

}

