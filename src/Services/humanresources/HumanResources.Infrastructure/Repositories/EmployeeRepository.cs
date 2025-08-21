using Microsoft.EntityFrameworkCore;
using TossErp.HumanResources.Domain.Aggregates.EmployeeAggregate;
using TossErp.HumanResources.Domain.ValueObjects;
using TossErp.HumanResources.Application.Employees.Commands;
using TossErp.HumanResources.Infrastructure.Data;

namespace TossErp.HumanResources.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly HumanResourcesContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public EmployeeRepository(HumanResourcesContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.Contacts)
            .Include(e => e.Qualifications)
            .Include(e => e.Experiences)
            .Include(e => e.Skills)
            .Include(e => e.Documents)
            .Include(e => e.SalaryHistories)
            .Include(e => e.PerformanceReviews)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<Employee?> GetByEmployeeNumberAsync(EmployeeNumber employeeNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.Contacts)
            .Include(e => e.Qualifications)
            .Include(e => e.Experiences)
            .Include(e => e.Skills)
            .Include(e => e.Documents)
            .Include(e => e.SalaryHistories)
            .Include(e => e.PerformanceReviews)
            .FirstOrDefaultAsync(e => e.EmployeeNumber.Value == employeeNumber.Value, cancellationToken);
    }

    public async Task<IEnumerable<Employee>> GetByDepartmentAsync(string department, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Where(e => e.Department == department)
            .Include(e => e.Contacts)
            .Include(e => e.Qualifications)
            .Include(e => e.Experiences)
            .Include(e => e.Skills)
            .Include(e => e.Documents)
            .Include(e => e.SalaryHistories)
            .Include(e => e.PerformanceReviews)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Employee>> GetByManagerAsync(Guid managerId, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Where(e => e.ManagerId == managerId)
            .Include(e => e.Contacts)
            .Include(e => e.Qualifications)
            .Include(e => e.Experiences)
            .Include(e => e.Skills)
            .Include(e => e.Documents)
            .Include(e => e.SalaryHistories)
            .Include(e => e.PerformanceReviews)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Employee>> GetActiveEmployeesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Where(e => e.Status == Domain.Enums.EmployeeStatus.Active)
            .Include(e => e.Contacts)
            .Include(e => e.Qualifications)
            .Include(e => e.Experiences)
            .Include(e => e.Skills)
            .Include(e => e.Documents)
            .Include(e => e.SalaryHistories)
            .Include(e => e.PerformanceReviews)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        await _context.Employees.AddAsync(employee, cancellationToken);
    }

    public void Update(Employee employee)
    {
        _context.Entry(employee).State = EntityState.Modified;
    }

    public void Remove(Employee employee)
    {
        _context.Employees.Remove(employee);
    }
}
