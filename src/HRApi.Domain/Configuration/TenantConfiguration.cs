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
    /// The unique name of this tenant instance (used for instance-specific customizations)
    /// </summary>
    public string InstanceName { get; set; } = "Default";
    
    /// <summary>
    /// Optional description of this tenant
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Custom business rules specific to this tenant instance
    /// Key-value pairs for flexible configuration
    /// </summary>
    public Dictionary<string, object> CustomRules { get; set; } = new();
    
    /// <summary>
    /// Feature flags for this tenant
    /// </summary>
    public Dictionary<string, bool> FeatureFlags { get; set; } = new();
}
