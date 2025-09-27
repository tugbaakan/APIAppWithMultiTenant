using HRApi.Application.DTOs.Employee;

namespace HRApi.Application.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
    Task<EmployeeDto?> GetEmployeeByIdAsync(Guid id);
    Task<EmployeeDto?> GetEmployeeByEmployeeNumberAsync(string employeeNumber);
    Task<IEnumerable<EmployeeDto>> GetEmployeesByDepartmentAsync(Guid departmentId);
    Task<IEnumerable<EmployeeDto>> GetEmployeesByManagerAsync(Guid managerId);
    Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto);
    Task<EmployeeDto> UpdateEmployeeAsync(Guid id, UpdateEmployeeDto updateEmployeeDto);
    Task<bool> DeleteEmployeeAsync(Guid id);
    Task<bool> EmployeeExistsAsync(Guid id);
    Task<bool> EmployeeNumberExistsAsync(string employeeNumber);
    Task<bool> EmailExistsAsync(string email);
}
