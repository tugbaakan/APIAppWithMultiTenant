using HRApi.Application.Strategies;
using HRApi.Domain.Configuration;
using HRApi.Domain.Interfaces;

namespace HRApi.Application.Factories;

/// <summary>
/// Factory for creating tenant-specific customization strategies
/// Supports both type-based and instance-based customizations
/// </summary>
public class TenantCustomizationFactory
{
    private readonly List<ITenantCustomizationStrategy> _strategies;

    public TenantCustomizationFactory()
    {
        // Register strategies in priority order (most specific first)
        _strategies = new List<ITenantCustomizationStrategy>
        {
            // Instance-specific strategies (highest priority)
            new Company1CustomStrategy(),
            new Company2CustomStrategy(),
            
            // Type-based strategies (fallback)
            new RestrictedTenantStrategy(),
            new StandardTenantStrategy()
        };
    }

    /// <summary>
    /// Get the appropriate customization strategy for the given tenant configuration
    /// </summary>
    public ITenantCustomizationStrategy GetStrategy(TenantConfiguration tenantConfig)
    {
        var tenantName = tenantConfig.InstanceName;
        var tenantType = tenantConfig.TenantType.ToString();

        // Find the first strategy that applies (most specific wins)
        var strategy = _strategies.FirstOrDefault(s => s.AppliesTo(tenantName, tenantType));
        
        return strategy ?? new StandardTenantStrategy(); // Fallback to standard
    }

    /// <summary>
    /// Register a new custom strategy (useful for adding tenant-specific strategies at runtime)
    /// </summary>
    public void RegisterStrategy(ITenantCustomizationStrategy strategy)
    {
        // Insert at the beginning to give it highest priority
        _strategies.Insert(0, strategy);
    }
}
