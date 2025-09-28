using HRApi.Domain.Common;
using HRApi.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Domain.Entities.HR;

public class Employee : BaseEntity
{
    [Required]
    [MaxLength(20)]
    public string EmployeeNumber { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? MiddleName { get; set; }
    
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(15)]
    public string? PhoneNumber { get; set; }
    
    public DateTime? DateOfBirth { get; set; }
    
    public Gender? Gender { get; set; }
    
    [MaxLength(200)]
    public string? Address { get; set; }
    
    [Required]
    public DateTime HireDate { get; set; }
    
    public DateTime? TerminationDate { get; set; }
    
    [Required]
    public Guid DepartmentId { get; set; }
    
    [Required]
    public Guid PositionId { get; set; }
    
    public Guid? ManagerId { get; set; }
    
    public EmploymentStatus Status { get; set; }
    
    public EmploymentType EmploymentType { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Salary { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    public bool IsDepartmentManager { get; set; }
    
    // Navigation properties
    public virtual Department Department { get; set; } = null!;
    public virtual Position Position { get; set; } = null!;
    public virtual Employee? Manager { get; set; }
    public virtual ICollection<Employee> Subordinates { get; set; } = new List<Employee>();
    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    public virtual ICollection<LeaveBalance> LeaveBalances { get; set; } = new List<LeaveBalance>();
    public virtual ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
    public virtual ICollection<Payslip> Payslips { get; set; } = new List<Payslip>();

    public Employee()
    {
        Status = EmploymentStatus.Active;
        EmploymentType = EmploymentType.FullTime;
        HireDate = DateTime.UtcNow;
    }
    
    public string FullName => $"{FirstName} {LastName}";
}
