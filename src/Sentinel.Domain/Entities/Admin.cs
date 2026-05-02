using Sentinel.Domain.Interfaces;

namespace Sentinel.Domain.Entities;

public sealed class PlatformAdmin : IActivatable 
{
    public Guid PlatformAdminId { get; set; }

    public required string AdminFulllName { get; set; }

    public required string Email { get; set; }

    public string? Permissions { get; set; }

    public bool IsActive { get; set; } = true;

}