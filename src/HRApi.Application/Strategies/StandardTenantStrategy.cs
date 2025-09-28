using HRApi.Domain.Entities.HR;
using HRApi.Domain.Enums;
using HRApi.Domain.Interfaces;

namespace HRApi.Application.Strategies;

/// <summary>
/// Default strategy for Standard tenant type
/// </summary>
public class StandardTenantStrategy : ITenantCustomizationStrategy
{
    public virtual IQueryable<Employee> ApplyEmployeeFiltering(IQueryable<Employee> employees)
    {
        // Standard tenants see all employees by default
        return employees;
    }

    public virtual void ApplyEmployeeCreationRules(Employee employee)
    {
        // No special rules for standard tenants
    }

    public virtual void ApplyEmployeeUpdateRules(Employee employee)
    {
        // No special rules for standard tenants
    }

    public virtual bool AppliesTo(string tenantName, string tenantType)
    {
        return tenantType.Equals(TenantType.Standard.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}
