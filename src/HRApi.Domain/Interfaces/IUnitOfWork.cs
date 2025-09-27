using HRApi.Domain.Entities.HR;

namespace HRApi.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Employee> Employees { get; }
    IRepository<Department> Departments { get; }
    IRepository<Position> Positions { get; }
    IRepository<LeaveType> LeaveTypes { get; }
    IRepository<LeaveRequest> LeaveRequests { get; }
    IRepository<LeaveBalance> LeaveBalances { get; }
    IRepository<Payslip> Payslips { get; }
    IRepository<Evaluation> Evaluations { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
