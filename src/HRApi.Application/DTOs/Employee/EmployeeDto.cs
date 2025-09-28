using HRApi.Domain.Enums;

namespace HRApi.Application.DTOs.Employee;

public class EmployeeDto
{
    public Guid Id { get; set; }
    
    public string FullName { get; set; } = string.Empty;
 
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public Guid PositionId { get; set; }
    public string PositionTitle { get; set; } = string.Empty;
}
