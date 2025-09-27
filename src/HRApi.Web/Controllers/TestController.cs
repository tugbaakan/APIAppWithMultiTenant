using Microsoft.AspNetCore.Mvc;
using HRApi.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace HRApi.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;

    public TestController(ILogger<TestController> logger)
    {
        _logger = logger;
    }

    [HttpGet("direct-db")]
    public async Task<ActionResult> TestDirectDB()
    {
        try
        {
            // Test direct connection to tenant database
            var connectionString = "Server=localhost\\MSSQLSERVER01;Database=HRApi_Tenant_Demo;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true";
            
            var optionsBuilder = new DbContextOptionsBuilder<HRDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            
            using var context = new HRDbContext(optionsBuilder.Options);
            
            // Test simple query without includes
            var employeeCount = await context.Employees.CountAsync();
            var departmentCount = await context.Departments.CountAsync();
            
            // Try to get simple employee data
            var employees = await context.Employees
                .Select(e => new {
                    e.Id,
                    e.EmployeeNumber,
                    e.FirstName,
                    e.LastName,
                    e.Email
                })
                .Take(5)
                .ToListAsync();
            
            return Ok(new {
                Status = "Success",
                EmployeeCount = employeeCount,
                DepartmentCount = departmentCount,
                SampleEmployees = employees
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in direct DB test");
            return StatusCode(500, new {
                Status = "Error",
                Message = ex.Message,
                StackTrace = ex.StackTrace
            });
        }
    }
}
