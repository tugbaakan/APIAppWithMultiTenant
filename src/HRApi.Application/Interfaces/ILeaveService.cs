using HRApi.Application.DTOs.Leave;
using HRApi.Domain.Enums;

namespace HRApi.Application.Interfaces;

public interface ILeaveService
{
    Task<IEnumerable<LeaveRequestDto>> GetAllLeaveRequestsAsync();
    Task<LeaveRequestDto?> GetLeaveRequestByIdAsync(Guid id);
    Task<IEnumerable<LeaveRequestDto>> GetLeaveRequestsByEmployeeAsync(Guid employeeId);
    Task<IEnumerable<LeaveRequestDto>> GetLeaveRequestsByStatusAsync(LeaveRequestStatus status);
    Task<LeaveRequestDto> CreateLeaveRequestAsync(CreateLeaveRequestDto createLeaveRequestDto);
    Task<LeaveRequestDto> ApproveLeaveRequestAsync(Guid id, Guid approvedBy, string? comments = null);
    Task<LeaveRequestDto> RejectLeaveRequestAsync(Guid id, Guid rejectedBy, string rejectionReason);
    Task<bool> CancelLeaveRequestAsync(Guid id);
    Task<bool> DeleteLeaveRequestAsync(Guid id);
}
