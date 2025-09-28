using HRApi.Domain.Entities.HR;
using HRApi.Domain.Enums;
using HRApi.Domain.Interfaces;

namespace HRApi.Application.Strategies;

/// <summary>
/// Custom strategy specifically for Company1
/// Inherits from StandardTenantStrategy but adds Company1-specific rules
/// </summary>
public class Company1CustomStrategy : StandardTenantStrategy
{
    public override IQueryable<Employee> ApplyEmployeeFiltering(IQueryable<Employee> employees)
    {
        // Company1 specific: Exclude contractors from standard employee lists
       return employees.Where(e => e.EmploymentType != EmploymentType.Contract);
    }

    public override void ApplyEmployeeCreationRules(Employee employee)
    {
        base.ApplyEmployeeCreationRules(employee);
        
        // Company1 specific rule: Employee numbers must start with "C1-"
        if (!employee.EmployeeNumber.StartsWith("C1"))
        {
            throw new InvalidOperationException("Company1 employee numbers must start with 'C1'");
        }
    }

    public override bool AppliesTo(string tenantName, string tenantType)
    {
        return tenantName.Equals("Company1", StringComparison.OrdinalIgnoreCase);
    }
}
