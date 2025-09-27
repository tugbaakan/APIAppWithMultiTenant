using HRApi.Domain.Common;
using HRApi.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace HRApi.Domain.Entities.HR;

public class LeaveRequest : BaseEntity
{
    [Required]
    public Guid EmployeeId { get; set; }
    
    [Required]
    public Guid LeaveTypeId { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    [Required]
    public int TotalDays { get; set; }
    
    [MaxLength(500)]
    public string? Reason { get; set; }
    
    [Required]
    public LeaveRequestStatus Status { get; set; }
    
    public DateTime? ApprovedAt { get; set; }
    
    public Guid? ApprovedBy { get; set; }
    
    [MaxLength(500)]
    public string? ApprovalComments { get; set; }
    
    public DateTime? RejectedAt { get; set; }
    
    public Guid? RejectedBy { get; set; }
    
    [MaxLength(500)]
    public string? RejectionReason { get; set; }
    
    // Navigation properties
    public virtual Employee Employee { get; set; } = null!;
    public virtual LeaveType LeaveType { get; set; } = null!;
    public virtual Employee? ApprovedByEmployee { get; set; }
    public virtual Employee? RejectedByEmployee { get; set; }

    public LeaveRequest()
    {
        Status = LeaveRequestStatus.Pending;
    }
}
