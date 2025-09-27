using HRApi.Application.DTOs.Employee;
using HRApi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRApi.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(IEmployeeService employeeService, ILogger<EmployeesController> logger)
    {
        _employeeService = employeeService;
        _logger = logger;
    }

    /// <summary>
    /// Get all employees
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAllEmployees()
    {
        try
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving employees");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get employee by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeDto>> GetEmployee(Guid id)
    {
        try
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound($"Employee with ID {id} not found");
            }
            return Ok(employee);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving employee {EmployeeId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get employee by employee number
    /// </summary>
    [HttpGet("by-number/{employeeNumber}")]
    public async Task<ActionResult<EmployeeDto>> GetEmployeeByNumber(string employeeNumber)
    {
        try
        {
            var employee = await _employeeService.GetEmployeeByEmployeeNumberAsync(employeeNumber);
            if (employee == null)
            {
                return NotFound($"Employee with number {employeeNumber} not found");
            }
            return Ok(employee);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving employee by number {EmployeeNumber}", employeeNumber);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get employees by department
    /// </summary>
    [HttpGet("by-department/{departmentId}")]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployeesByDepartment(Guid departmentId)
    {
        try
        {
            var employees = await _employeeService.GetEmployeesByDepartmentAsync(departmentId);
            return Ok(employees);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving employees by department {DepartmentId}", departmentId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new employee
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> CreateEmployee([FromBody] CreateEmployeeDto createEmployeeDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var employee = await _employeeService.CreateEmployeeAsync(createEmployeeDto);
            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while creating employee");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating employee");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update an existing employee
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<EmployeeDto>> UpdateEmployee(Guid id, [FromBody] UpdateEmployeeDto updateEmployeeDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var employee = await _employeeService.UpdateEmployeeAsync(id, updateEmployeeDto);
            return Ok(employee);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Employee not found for update: {EmployeeId}", id);
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while updating employee {EmployeeId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating employee {EmployeeId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete an employee
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteEmployee(Guid id)
    {
        try
        {
            var success = await _employeeService.DeleteEmployeeAsync(id);
            if (!success)
            {
                return NotFound($"Employee with ID {id} not found");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting employee {EmployeeId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
