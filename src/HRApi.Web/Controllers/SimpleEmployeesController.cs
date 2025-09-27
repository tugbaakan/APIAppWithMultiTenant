using Microsoft.AspNetCore.Mvc;
using HRApi.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace HRApi.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SimpleEmployeesController : ControllerBase
{
    private readonly HRDbContext _context;
    private readonly ILogger<SimpleEmployeesController> _logger;

    public SimpleEmployeesController(HRDbContext context, ILogger<SimpleEmployeesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult> GetEmployees()
    {
        try
        {
            _logger.LogInformation("Getting employees with tenant context");
            
            // Simple query without includes to test if tenant context works
            var employees = await _context.Employees
                .Select(e => new {
                    e.Id,
                    e.EmployeeNumber,
                    e.FirstName,
                    e.LastName,
                    e.Email,
                    e.Status,
                    e.HireDate
                })
                .ToListAsync();
            
            _logger.LogInformation("Found {Count} employees", employees.Count);
            
            return Ok(new {
                Success = true,
                Count = employees.Count,
                Data = employees
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting employees");
            return StatusCode(500, new {
                Success = false,
                Error = ex.Message,
                StackTrace = ex.StackTrace
            });
        }
    }
}
