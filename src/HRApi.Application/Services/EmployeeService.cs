using AutoMapper;
using HRApi.Application.DTOs.Employee;
using HRApi.Application.Interfaces;
using HRApi.Domain.Entities.HR;
using HRApi.Domain.Interfaces;

namespace HRApi.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
    {
        var employees = await _unitOfWork.Employees.GetAllAsync();
        return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
    }

    public async Task<EmployeeDto?> GetEmployeeByIdAsync(Guid id)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id);
        return employee != null ? _mapper.Map<EmployeeDto>(employee) : null;
    }

    public async Task<EmployeeDto?> GetEmployeeByEmployeeNumberAsync(string employeeNumber)
    {
        var employee = await _unitOfWork.Employees.GetFirstOrDefaultAsync(e => e.EmployeeNumber == employeeNumber);
        return employee != null ? _mapper.Map<EmployeeDto>(employee) : null;
    }

    public async Task<IEnumerable<EmployeeDto>> GetEmployeesByDepartmentAsync(Guid departmentId)
    {
        var employees = await _unitOfWork.Employees.GetAsync(e => e.DepartmentId == departmentId);
        return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
    }

    public async Task<IEnumerable<EmployeeDto>> GetEmployeesByManagerAsync(Guid managerId)
    {
        var employees = await _unitOfWork.Employees.GetAsync(e => e.ManagerId == managerId);
        return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
    }

    public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto)
    {
        // Check if employee number already exists
        if (await EmployeeNumberExistsAsync(createEmployeeDto.EmployeeNumber))
        {
            throw new InvalidOperationException($"Employee number '{createEmployeeDto.EmployeeNumber}' already exists");
        }

        // Check if email already exists
        if (await EmailExistsAsync(createEmployeeDto.Email))
        {
            throw new InvalidOperationException($"Email '{createEmployeeDto.Email}' already exists");
        }

        var employee = _mapper.Map<Employee>(createEmployeeDto);
        
        await _unitOfWork.Employees.AddAsync(employee);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<EmployeeDto>(employee);
    }

    public async Task<EmployeeDto> UpdateEmployeeAsync(Guid id, UpdateEmployeeDto updateEmployeeDto)
    {
        var existingEmployee = await _unitOfWork.Employees.GetByIdAsync(id);
        if (existingEmployee == null)
        {
            throw new KeyNotFoundException($"Employee with ID {id} not found");
        }

        // Check if email already exists for another employee
        var emailExists = await _unitOfWork.Employees.ExistsAsync(e => e.Email == updateEmployeeDto.Email && e.Id != id);
        if (emailExists)
        {
            throw new InvalidOperationException($"Email '{updateEmployeeDto.Email}' already exists");
        }

        _mapper.Map(updateEmployeeDto, existingEmployee);
        
        await _unitOfWork.Employees.UpdateAsync(existingEmployee);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<EmployeeDto>(existingEmployee);
    }

    public async Task<bool> DeleteEmployeeAsync(Guid id)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id);
        if (employee == null)
        {
            return false;
        }

        await _unitOfWork.Employees.DeleteAsync(employee);
        await _unitOfWork.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> EmployeeExistsAsync(Guid id)
    {
        return await _unitOfWork.Employees.ExistsAsync(e => e.Id == id);
    }

    public async Task<bool> EmployeeNumberExistsAsync(string employeeNumber)
    {
        return await _unitOfWork.Employees.ExistsAsync(e => e.EmployeeNumber == employeeNumber);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _unitOfWork.Employees.ExistsAsync(e => e.Email == email);
    }
}
