using HRApi.Domain.Entities.HR;
using HRApi.Domain.Interfaces;
using HRApi.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore.Storage;

namespace HRApi.Infrastructure.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly HRDbContext _context;
    private IDbContextTransaction? _transaction;

    private IRepository<Employee>? _employees;
    private IRepository<Department>? _departments;
    private IRepository<Position>? _positions;
    private IRepository<LeaveType>? _leaveTypes;
    private IRepository<LeaveRequest>? _leaveRequests;
    private IRepository<LeaveBalance>? _leaveBalances;
    private IRepository<Payslip>? _payslips;
    private IRepository<Evaluation>? _evaluations;

    public UnitOfWork(HRDbContext context)
    {
        _context = context;
    }

    public IRepository<Employee> Employees => _employees ??= new Repository<Employee>(_context);
    public IRepository<Department> Departments => _departments ??= new Repository<Department>(_context);
    public IRepository<Position> Positions => _positions ??= new Repository<Position>(_context);
    public IRepository<LeaveType> LeaveTypes => _leaveTypes ??= new Repository<LeaveType>(_context);
    public IRepository<LeaveRequest> LeaveRequests => _leaveRequests ??= new Repository<LeaveRequest>(_context);
    public IRepository<LeaveBalance> LeaveBalances => _leaveBalances ??= new Repository<LeaveBalance>(_context);
    public IRepository<Payslip> Payslips => _payslips ??= new Repository<Payslip>(_context);
    public IRepository<Evaluation> Evaluations => _evaluations ??= new Repository<Evaluation>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
