using HRApi.Domain.Entities.HR;

namespace HRApi.Domain.Interfaces;

/// <summary>
/// Interface for tenant-specific customization strategies
/// </summary>
public interface ITenantCustomizationStrategy
{
    /// <summary>
    /// Apply tenant-specific filtering to employee queries
    /// </summary>
    IQueryable<Employee> ApplyEmployeeFiltering(IQueryable<Employee> employees);
    
    /// <summary>
    /// Apply tenant-specific business rules during employee creation
    /// </summary>
    void ApplyEmployeeCreationRules(Employee employee);
    
    /// <summary>
    /// Apply tenant-specific business rules during employee updates
    /// </summary>
    void ApplyEmployeeUpdateRules(Employee employee);
    
    /// <summary>
    /// Check if this strategy applies to the given tenant configuration
    /// </summary>
    bool AppliesTo(string tenantName, string tenantType);
}
