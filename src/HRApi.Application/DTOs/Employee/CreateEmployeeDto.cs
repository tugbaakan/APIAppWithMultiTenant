using HRApi.Domain.Enums;

namespace HRApi.Application.DTOs.Employee;

public class CreateEmployeeDto
{
    public string EmployeeNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public string? Address { get; set; }
    public DateTime HireDate { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid PositionId { get; set; }
    public Guid? ManagerId { get; set; }
    public EmploymentStatus Status { get; set; }
    public EmploymentType EmploymentType { get; set; }
    public decimal? Salary { get; set; }
    public string? Notes { get; set; }
}
