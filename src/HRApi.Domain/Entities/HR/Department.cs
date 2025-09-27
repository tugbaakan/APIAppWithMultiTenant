using HRApi.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace HRApi.Domain.Entities.HR;

public class Department : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [MaxLength(10)]
    public string? Code { get; set; }
    
    public Guid? ParentDepartmentId { get; set; }
    
    public Guid? ManagerId { get; set; }
    
    public bool IsActive { get; set; }
    
    // Navigation properties
    public virtual Department? ParentDepartment { get; set; }
    public virtual ICollection<Department> SubDepartments { get; set; } = new List<Department>();
    public virtual Employee? Manager { get; set; }
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public Department()
    {
        IsActive = true;
    }
}
