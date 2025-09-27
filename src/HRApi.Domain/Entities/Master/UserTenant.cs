using HRApi.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace HRApi.Domain.Entities.Master;

public class UserTenant : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    public Guid TenantId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Role { get; set; } = string.Empty;
    
    public bool IsActive { get; set; }
    
    public DateTime? LastLoginAt { get; set; }
    
    public virtual Tenant Tenant { get; set; } = null!;

    public UserTenant()
    {
        IsActive = true;
    }
}
