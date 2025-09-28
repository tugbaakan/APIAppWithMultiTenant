namespace HRApi.Domain.Enums;

/// <summary>
/// Defines the types of tenants in the system.
/// This enum provides compile-time safety for tenant-specific business logic
/// while maintaining the flexibility of database-driven tenant management.
/// </summary>
public enum TenantType
{
    /// <summary>
    /// Standard company with full employee access
    /// </summary>
    Standard = 1,
    
    /// <summary>
    /// Company with restricted access (managers only)
    /// </summary>
    Restricted = 2,
    
    /// <summary>
    /// Enterprise company with advanced features
    /// </summary>
    Enterprise = 3,
    
    /// <summary>
    /// Trial company with limited features
    /// </summary>
    Trial = 4
}
