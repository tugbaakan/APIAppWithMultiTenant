using HRApi.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Domain.Entities.HR;

public class Payslip : BaseEntity
{
    [Required]
    public Guid EmployeeId { get; set; }
    
    [Required]
    public int Year { get; set; }
    
    [Required]
    public int Month { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal BasicSalary { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Allowances { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Overtime { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Bonus { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal GrossSalary { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? TaxDeduction { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? InsuranceDeduction { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? OtherDeductions { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalDeductions { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal NetSalary { get; set; }
    
    public DateTime PayDate { get; set; }
    
    public bool IsPaid { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    // Navigation properties
    public virtual Employee Employee { get; set; } = null!;

    public Payslip()
    {
        Year = DateTime.UtcNow.Year;
        Month = DateTime.UtcNow.Month;
        IsPaid = false;
        Allowances = 0;
        Overtime = 0;
        Bonus = 0;
        TaxDeduction = 0;
        InsuranceDeduction = 0;
        OtherDeductions = 0;
    }
}
