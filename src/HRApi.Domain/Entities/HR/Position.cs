using HRApi.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace HRApi.Domain.Entities.HR;

public class Position : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [MaxLength(10)]
    public string? Code { get; set; }
    
    public decimal? MinSalary { get; set; }
    
    public decimal? MaxSalary { get; set; }
    
    public bool IsActive { get; set; }
    
    [MaxLength(50)]
    public string? Level { get; set; } // Junior, Mid, Senior, etc.
    
    // Navigation properties
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public Position()
    {
        IsActive = true;
    }
}
