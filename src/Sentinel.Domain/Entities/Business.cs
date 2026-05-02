using Sentinel.Domain.Interfaces;

namespace Sentinel.Domain.Entities;

    

public sealed class Business : IActivatable
{
    public Guid BusinessId { get; set; }
    public required string BusinessName { get; set; }
    public string OwnerRealPhone { get; set; } = null!;
    public string VirtualBusinessPhone { get; set; } = null!;  
    public AfterHoursMode AfterHoursMode { get; set; } = AfterHoursMode.Disabled;
    public bool RecordCalls { get; set; }
    public bool IsActive { get; set; }
    public ICollection<BusinessHours> Hours { get; set; } = [];

    public ICollection<Call> Calls { get; set; } = [];

}

