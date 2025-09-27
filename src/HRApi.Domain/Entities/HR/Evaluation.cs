using HRApi.Domain.Common;
using HRApi.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Domain.Entities.HR;

public class Evaluation : BaseEntity
{
    [Required]
    public Guid EmployeeId { get; set; }
    
    [Required]
    public Guid EvaluatorId { get; set; }
    
    [Required]
    public DateTime EvaluationPeriodStart { get; set; }
    
    [Required]
    public DateTime EvaluationPeriodEnd { get; set; }
    
    [Required]
    public EvaluationType Type { get; set; }
    
    [Required]
    public EvaluationStatus Status { get; set; }
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal? OverallScore { get; set; }
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal? TechnicalSkillsScore { get; set; }
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal? CommunicationScore { get; set; }
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal? TeamworkScore { get; set; }
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal? LeadershipScore { get; set; }
    
    [MaxLength(2000)]
    public string? Strengths { get; set; }
    
    [MaxLength(2000)]
    public string? AreasForImprovement { get; set; }
    
    [MaxLength(2000)]
    public string? Goals { get; set; }
    
    [MaxLength(2000)]
    public string? EmployeeComments { get; set; }
    
    [MaxLength(2000)]
    public string? EvaluatorComments { get; set; }
    
    public DateTime? CompletedAt { get; set; }
    
    // Navigation properties
    public virtual Employee Employee { get; set; } = null!;
    public virtual Employee Evaluator { get; set; } = null!;

    public Evaluation()
    {
        Status = EvaluationStatus.Draft;
        Type = EvaluationType.Annual;
    }
}
