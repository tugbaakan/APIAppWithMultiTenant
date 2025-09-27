using AutoMapper;
using HRApi.Application.DTOs.Leave;
using HRApi.Application.Interfaces;
using HRApi.Domain.Entities.HR;
using HRApi.Domain.Enums;
using HRApi.Domain.Interfaces;

namespace HRApi.Application.Services;

public class LeaveService : ILeaveService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public LeaveService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LeaveRequestDto>> GetAllLeaveRequestsAsync()
    {
        var leaveRequests = await _unitOfWork.LeaveRequests.GetAllAsync();
        return _mapper.Map<IEnumerable<LeaveRequestDto>>(leaveRequests);
    }

    public async Task<LeaveRequestDto?> GetLeaveRequestByIdAsync(Guid id)
    {
        var leaveRequest = await _unitOfWork.LeaveRequests.GetByIdAsync(id);
        return leaveRequest != null ? _mapper.Map<LeaveRequestDto>(leaveRequest) : null;
    }

    public async Task<IEnumerable<LeaveRequestDto>> GetLeaveRequestsByEmployeeAsync(Guid employeeId)
    {
        var leaveRequests = await _unitOfWork.LeaveRequests.GetAsync(lr => lr.EmployeeId == employeeId);
        return _mapper.Map<IEnumerable<LeaveRequestDto>>(leaveRequests);
    }

    public async Task<IEnumerable<LeaveRequestDto>> GetLeaveRequestsByStatusAsync(LeaveRequestStatus status)
    {
        var leaveRequests = await _unitOfWork.LeaveRequests.GetAsync(lr => lr.Status == status);
        return _mapper.Map<IEnumerable<LeaveRequestDto>>(leaveRequests);
    }

    public async Task<LeaveRequestDto> CreateLeaveRequestAsync(CreateLeaveRequestDto createLeaveRequestDto)
    {
        // Validate employee exists
        var employeeExists = await _unitOfWork.Employees.ExistsAsync(e => e.Id == createLeaveRequestDto.EmployeeId);
        if (!employeeExists)
        {
            throw new KeyNotFoundException($"Employee with ID {createLeaveRequestDto.EmployeeId} not found");
        }

        // Validate leave type exists
        var leaveTypeExists = await _unitOfWork.LeaveTypes.ExistsAsync(lt => lt.Id == createLeaveRequestDto.LeaveTypeId);
        if (!leaveTypeExists)
        {
            throw new KeyNotFoundException($"Leave type with ID {createLeaveRequestDto.LeaveTypeId} not found");
        }

        // Check for overlapping leave requests
        var hasOverlappingRequest = await _unitOfWork.LeaveRequests.ExistsAsync(lr => 
            lr.EmployeeId == createLeaveRequestDto.EmployeeId &&
            lr.Status != LeaveRequestStatus.Rejected &&
            lr.Status != LeaveRequestStatus.Cancelled &&
            ((lr.StartDate <= createLeaveRequestDto.EndDate && lr.EndDate >= createLeaveRequestDto.StartDate)));

        if (hasOverlappingRequest)
        {
            throw new InvalidOperationException("There is already a leave request for the selected dates");
        }

        var leaveRequest = _mapper.Map<LeaveRequest>(createLeaveRequestDto);
        leaveRequest.Status = LeaveRequestStatus.Pending;

        await _unitOfWork.LeaveRequests.AddAsync(leaveRequest);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<LeaveRequestDto>(leaveRequest);
    }

    public async Task<LeaveRequestDto> ApproveLeaveRequestAsync(Guid id, Guid approvedBy, string? comments = null)
    {
        var leaveRequest = await _unitOfWork.LeaveRequests.GetByIdAsync(id);
        if (leaveRequest == null)
        {
            throw new KeyNotFoundException($"Leave request with ID {id} not found");
        }

        if (leaveRequest.Status != LeaveRequestStatus.Pending)
        {
            throw new InvalidOperationException($"Leave request is not in pending status. Current status: {leaveRequest.Status}");
        }

        leaveRequest.Status = LeaveRequestStatus.Approved;
        leaveRequest.ApprovedBy = approvedBy;
        leaveRequest.ApprovedAt = DateTime.UtcNow;
        leaveRequest.ApprovalComments = comments;

        await _unitOfWork.LeaveRequests.UpdateAsync(leaveRequest);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<LeaveRequestDto>(leaveRequest);
    }

    public async Task<LeaveRequestDto> RejectLeaveRequestAsync(Guid id, Guid rejectedBy, string rejectionReason)
    {
        var leaveRequest = await _unitOfWork.LeaveRequests.GetByIdAsync(id);
        if (leaveRequest == null)
        {
            throw new KeyNotFoundException($"Leave request with ID {id} not found");
        }

        if (leaveRequest.Status != LeaveRequestStatus.Pending)
        {
            throw new InvalidOperationException($"Leave request is not in pending status. Current status: {leaveRequest.Status}");
        }

        leaveRequest.Status = LeaveRequestStatus.Rejected;
        leaveRequest.RejectedBy = rejectedBy;
        leaveRequest.RejectedAt = DateTime.UtcNow;
        leaveRequest.RejectionReason = rejectionReason;

        await _unitOfWork.LeaveRequests.UpdateAsync(leaveRequest);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<LeaveRequestDto>(leaveRequest);
    }

    public async Task<bool> CancelLeaveRequestAsync(Guid id)
    {
        var leaveRequest = await _unitOfWork.LeaveRequests.GetByIdAsync(id);
        if (leaveRequest == null)
        {
            return false;
        }

        if (leaveRequest.Status != LeaveRequestStatus.Pending && leaveRequest.Status != LeaveRequestStatus.Approved)
        {
            throw new InvalidOperationException($"Cannot cancel leave request with status: {leaveRequest.Status}");
        }

        leaveRequest.Status = LeaveRequestStatus.Cancelled;

        await _unitOfWork.LeaveRequests.UpdateAsync(leaveRequest);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteLeaveRequestAsync(Guid id)
    {
        var leaveRequest = await _unitOfWork.LeaveRequests.GetByIdAsync(id);
        if (leaveRequest == null)
        {
            return false;
        }

        await _unitOfWork.LeaveRequests.DeleteAsync(leaveRequest);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
