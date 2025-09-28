using HRApi.Domain.Enums;

namespace HRApi.Domain.Configuration;

/// <summary>
/// Configuration for the current tenant instance.
/// This is set once per tenant deployment and doesn't change at runtime.
/// </summary>
public class TenantConfiguration
{
    /// <summary>
    /// The type of tenant this instance represents
    /// </summary>
    public TenantType TenantType { get; set; } = TenantType.Standard;
    
    /// <summary>
    /// The name of this tenant instance (for logging/display)
    /// </summary>
    public string InstanceName { get; set; } = "Default";
    
    /// <summary>
    /// Optional description of this tenant
    /// </summary>
    public string? Description { get; set; }
}
