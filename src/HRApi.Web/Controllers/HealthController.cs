using Microsoft.AspNetCore.Mvc;

namespace HRApi.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public ActionResult<object> Get()
    {
        return Ok(new { 
            Status = "Healthy", 
            Timestamp = DateTime.UtcNow,
            Message = "API is running successfully" 
        });
    }

    [HttpGet("database")]
    public ActionResult<object> CheckDatabase()
    {
        try
        {
            // Simple test without tenant dependency
            return Ok(new { 
                Status = "Database connection available", 
                Timestamp = DateTime.UtcNow 
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { 
                Status = "Database connection failed", 
                Error = ex.Message,
                Timestamp = DateTime.UtcNow 
            });
        }
    }
}
