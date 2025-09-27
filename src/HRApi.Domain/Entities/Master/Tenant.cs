using HRApi.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace HRApi.Domain.Entities.Master;

public class Tenant : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Subdomain { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string ConnectionString { get; set; } = string.Empty;
    
    public bool IsActive { get; set; }
    
    [MaxLength(2000)]
    public string? Settings { get; set; } // JSON configuration for tenant-specific features
    
    [MaxLength(100)]
    public string? ContactEmail { get; set; }
    
    [MaxLength(15)]
    public string? ContactPhone { get; set; }
    
    public DateTime? SubscriptionExpiry { get; set; }
    
    public virtual ICollection<UserTenant> UserTenants { get; set; } = new List<UserTenant>();

    public Tenant()
    {
        IsActive = true;
    }
}
