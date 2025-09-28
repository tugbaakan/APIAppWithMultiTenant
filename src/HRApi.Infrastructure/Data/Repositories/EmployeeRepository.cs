using HRApi.Domain.Entities.HR;
using HRApi.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HRApi.Infrastructure.Data.Repositories;

public class EmployeeRepository : Repository<Employee>
{
    public EmployeeRepository(HRDbContext context) : base(context)
    {
    }

    public override async Task<Employee?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Include(e => e.Manager)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public override async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return await _dbSet
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Include(e => e.Manager)
            .ToListAsync();
    }

    public override async Task<IEnumerable<Employee>> GetAsync(Expression<Func<Employee, bool>> predicate)
    {
        return await _dbSet
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Include(e => e.Manager)
            .Where(predicate)
            .ToListAsync();
    }

    public override async Task<Employee?> GetFirstOrDefaultAsync(Expression<Func<Employee, bool>> predicate)
    {
        return await _dbSet
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Include(e => e.Manager)
            .FirstOrDefaultAsync(predicate);
    }
}
