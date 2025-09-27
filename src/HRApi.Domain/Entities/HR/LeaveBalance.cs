using HRApi.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace HRApi.Domain.Entities.HR;

public class LeaveBalance : BaseEntity
{
    [Required]
    public Guid EmployeeId { get; set; }
    
    [Required]
    public Guid LeaveTypeId { get; set; }
    
    [Required]
    public int Year { get; set; }
    
    [Required]
    public int AllocatedDays { get; set; }
    
    [Required]
    public int UsedDays { get; set; }
    
    public int CarryForwardDays { get; set; }
    
    public int RemainingDays => AllocatedDays + CarryForwardDays - UsedDays;
    
    // Navigation properties
    public virtual Employee Employee { get; set; } = null!;
    public virtual LeaveType LeaveType { get; set; } = null!;

    public LeaveBalance()
    {
        Year = DateTime.UtcNow.Year;
        UsedDays = 0;
        CarryForwardDays = 0;
    }
}
