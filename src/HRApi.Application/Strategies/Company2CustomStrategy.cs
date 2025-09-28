using HRApi.Domain.Entities.HR;
using HRApi.Domain.Interfaces;

namespace HRApi.Application.Strategies;

/// <summary>
/// Custom strategy specifically for Company2
/// Inherits from RestrictedTenantStrategy but adds Company2-specific rules
/// </summary>
public class Company2CustomStrategy : RestrictedTenantStrategy
{
    public override IQueryable<Employee> ApplyEmployeeFiltering(IQueryable<Employee> employees)
    {
        // Company2 specific: Show managers AND senior employees (5+ years experience)
        return employees.Where(e => 
            e.IsDepartmentManager &&
            e.HireDate >= DateTime.Now.AddYears(-5));
    }

    public override void ApplyEmployeeCreationRules(Employee employee)
    {
        base.ApplyEmployeeCreationRules(employee);
        
        // Company2 specific rule: All new employees must have a manager assigned
        if (employee.ManagerId == null)
        {
            throw new InvalidOperationException("Company2 requires all employees to have a manager assigned");
        }
    }

    public override bool AppliesTo(string tenantName, string tenantType)
    {
        return tenantName.Equals("Company2", StringComparison.OrdinalIgnoreCase);
    }
}
