using HRApi.Application.DTOs.Leave;
using HRApi.Application.Interfaces;
using HRApi.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace HRApi.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaveRequestsController : ControllerBase
{
    private readonly ILeaveService _leaveService;
    private readonly ILogger<LeaveRequestsController> _logger;

    public LeaveRequestsController(ILeaveService leaveService, ILogger<LeaveRequestsController> logger)
    {
        _leaveService = leaveService;
        _logger = logger;
    }

    /// <summary>
    /// Get all leave requests
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LeaveRequestDto>>> GetAllLeaveRequests()
    {
        try
        {
            var leaveRequests = await _leaveService.GetAllLeaveRequestsAsync();
            return Ok(leaveRequests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving leave requests");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get leave request by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<LeaveRequestDto>> GetLeaveRequest(Guid id)
    {
        try
        {
            var leaveRequest = await _leaveService.GetLeaveRequestByIdAsync(id);
            if (leaveRequest == null)
            {
                return NotFound($"Leave request with ID {id} not found");
            }
            return Ok(leaveRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving leave request {LeaveRequestId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get leave requests by employee
    /// </summary>
    [HttpGet("by-employee/{employeeId}")]
    public async Task<ActionResult<IEnumerable<LeaveRequestDto>>> GetLeaveRequestsByEmployee(Guid employeeId)
    {
        try
        {
            var leaveRequests = await _leaveService.GetLeaveRequestsByEmployeeAsync(employeeId);
            return Ok(leaveRequests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving leave requests for employee {EmployeeId}", employeeId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get leave requests by status
    /// </summary>
    [HttpGet("by-status/{status}")]
    public async Task<ActionResult<IEnumerable<LeaveRequestDto>>> GetLeaveRequestsByStatus(LeaveRequestStatus status)
    {
        try
        {
            var leaveRequests = await _leaveService.GetLeaveRequestsByStatusAsync(status);
            return Ok(leaveRequests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving leave requests by status {Status}", status);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new leave request
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<LeaveRequestDto>> CreateLeaveRequest([FromBody] CreateLeaveRequestDto createLeaveRequestDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var leaveRequest = await _leaveService.CreateLeaveRequestAsync(createLeaveRequestDto);
            return CreatedAtAction(nameof(GetLeaveRequest), new { id = leaveRequest.Id }, leaveRequest);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Referenced entity not found while creating leave request");
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while creating leave request");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating leave request");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Approve a leave request
    /// </summary>
    [HttpPost("{id}/approve")]
    public async Task<ActionResult<LeaveRequestDto>> ApproveLeaveRequest(Guid id, [FromBody] ApproveLeaveRequestDto approveDto)
    {
        try
        {
            var leaveRequest = await _leaveService.ApproveLeaveRequestAsync(id, approveDto.ApprovedBy, approveDto.Comments);
            return Ok(leaveRequest);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Leave request not found for approval: {LeaveRequestId}", id);
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while approving leave request {LeaveRequestId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving leave request {LeaveRequestId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Reject a leave request
    /// </summary>
    [HttpPost("{id}/reject")]
    public async Task<ActionResult<LeaveRequestDto>> RejectLeaveRequest(Guid id, [FromBody] RejectLeaveRequestDto rejectDto)
    {
        try
        {
            var leaveRequest = await _leaveService.RejectLeaveRequestAsync(id, rejectDto.RejectedBy, rejectDto.RejectionReason);
            return Ok(leaveRequest);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Leave request not found for rejection: {LeaveRequestId}", id);
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while rejecting leave request {LeaveRequestId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting leave request {LeaveRequestId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Cancel a leave request
    /// </summary>
    [HttpPost("{id}/cancel")]
    public async Task<ActionResult> CancelLeaveRequest(Guid id)
    {
        try
        {
            var success = await _leaveService.CancelLeaveRequestAsync(id);
            if (!success)
            {
                return NotFound($"Leave request with ID {id} not found");
            }
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while cancelling leave request {LeaveRequestId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling leave request {LeaveRequestId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete a leave request
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteLeaveRequest(Guid id)
    {
        try
        {
            var success = await _leaveService.DeleteLeaveRequestAsync(id);
            if (!success)
            {
                return NotFound($"Leave request with ID {id} not found");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting leave request {LeaveRequestId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}

public class ApproveLeaveRequestDto
{
    public Guid ApprovedBy { get; set; }
    public string? Comments { get; set; }
}

public class RejectLeaveRequestDto
{
    public Guid RejectedBy { get; set; }
    public string RejectionReason { get; set; } = string.Empty;
}
