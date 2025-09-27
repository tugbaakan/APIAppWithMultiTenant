using AutoMapper;
using HRApi.Application.DTOs.Leave;
using HRApi.Domain.Entities.HR;

namespace HRApi.Application.Mappings;

public class LeaveProfile : Profile
{
    public LeaveProfile()
    {
        CreateMap<LeaveRequest, LeaveRequestDto>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName))
            .ForMember(dest => dest.LeaveTypeName, opt => opt.MapFrom(src => src.LeaveType.Name))
            .ForMember(dest => dest.ApprovedByName, opt => opt.MapFrom(src => src.ApprovedByEmployee != null ? src.ApprovedByEmployee.FullName : null))
            .ForMember(dest => dest.RejectedByName, opt => opt.MapFrom(src => src.RejectedByEmployee != null ? src.RejectedByEmployee.FullName : null));

        CreateMap<CreateLeaveRequestDto, LeaveRequest>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TotalDays, opt => opt.MapFrom(src => CalculateTotalDays(src.StartDate, src.EndDate)))
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
    }

    private static int CalculateTotalDays(DateTime startDate, DateTime endDate)
    {
        return (int)(endDate - startDate).TotalDays + 1;
    }
}
