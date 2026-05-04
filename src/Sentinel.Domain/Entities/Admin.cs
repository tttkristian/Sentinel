using Sentinel.Domain.Interfaces;

namespace Sentinel.Domain.Entities;

public sealed class Admin : IActivatable, IAuditable
{
    public Guid AdminId { get; set; }

    public required string AdminFullName { get; set; }

    public required string Email { get; set; }

    public string? Permissions { get; set; }

    public bool IsActive { get; set; } = true;
    public string AdminPhoneNumber { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    

}