using HRApi.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace HRApi.Domain.Entities.HR;

public class LeaveType : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? Description { get; set; }
    
    [Required]
    public int MaxDaysPerYear { get; set; }
    
    public bool RequiresApproval { get; set; }
    
    public bool IsCarryForward { get; set; }
    
    public int? MaxCarryForwardDays { get; set; }
    
    public bool IsActive { get; set; }
    
    [MaxLength(10)]
    public string? Code { get; set; }
    
    // Navigation properties
    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    public virtual ICollection<LeaveBalance> LeaveBalances { get; set; } = new List<LeaveBalance>();

    public LeaveType()
    {
        IsActive = true;
        RequiresApproval = true;
        IsCarryForward = false;
    }
}
