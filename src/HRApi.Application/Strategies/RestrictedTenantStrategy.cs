using HRApi.Domain.Entities.HR;
using HRApi.Domain.Enums;
using HRApi.Domain.Interfaces;

namespace HRApi.Application.Strategies;

/// <summary>
/// Strategy for Restricted tenant type - shows only department managers
/// </summary>
public class RestrictedTenantStrategy : ITenantCustomizationStrategy
{
    public virtual IQueryable<Employee> ApplyEmployeeFiltering(IQueryable<Employee> employees)
    {
        // Restricted tenants only see department managers
        return employees.Where(e => e.IsDepartmentManager);
    }

    public virtual void ApplyEmployeeCreationRules(Employee employee)
    {
        // Could add validation rules specific to restricted tenants
    }

    public virtual void ApplyEmployeeUpdateRules(Employee employee)
    {
        // Could add validation rules specific to restricted tenants
    }

    public virtual bool AppliesTo(string tenantName, string tenantType)
    {
        return tenantType.Equals(TenantType.Restricted.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}
