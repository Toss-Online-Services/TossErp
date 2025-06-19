using Microsoft.EntityFrameworkCore;
using TossErp.Domain.SeedWork;
using TossErp.POS.Domain.AggregatesModel.SaleAggregate;
using TossErp.POS.Domain.Enums;
using TossErp.POS.Infrastructure.Data;

namespace TossErp.POS.Infrastructure.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly POSDbContext _context;

        public SaleRepository(POSDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Sales
                .Include(s => s.SaleItems)
                .Include(s => s.Payments)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Sale>> ListAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Sales
                .Include(s => s.SaleItems)
                .Include(s => s.Payments)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Sale entity, CancellationToken cancellationToken = default)
        {
            await _context.Sales.AddAsync(entity, cancellationToken);
        }

        public void Update(Sale entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(Sale entity)
        {
            _context.Sales.Remove(entity);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Sale?> GetBySaleNumberAsync(string saleNumber)
        {
            return await _context.Sales
                .Include(s => s.SaleItems)
                .Include(s => s.Payments)
                .FirstOrDefaultAsync(s => s.SaleNumber == saleNumber);
        }

        public async Task<IEnumerable<Sale>> GetSalesByCustomerAsync(Guid customerId)
        {
            return await _context.Sales
                .Include(s => s.SaleItems)
                .Include(s => s.Payments)
                .Where(s => s.CustomerId == customerId)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sale>> GetSalesByCashierAsync(Guid cashierId)
        {
            return await _context.Sales
                .Include(s => s.SaleItems)
                .Include(s => s.Payments)
                .Where(s => s.CashierId == cashierId)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sale>> GetSalesByStatusAsync(SaleStatus status)
        {
            return await _context.Sales
                .Include(s => s.SaleItems)
                .Include(s => s.Payments)
                .Where(s => s.Status == status)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Sales
                .Include(s => s.SaleItems)
                .Include(s => s.Payments)
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sale>> GetCompletedSalesAsync()
        {
            return await _context.Sales
                .Include(s => s.SaleItems)
                .Include(s => s.Payments)
                .Where(s => s.Status == SaleStatus.Completed)
                .OrderByDescending(s => s.CompletedAt)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalSalesForPeriodAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Sales
                .Where(s => s.Status == SaleStatus.Completed && 
                           s.SaleDate >= startDate && s.SaleDate <= endDate)
                .SumAsync(s => s.TotalAmount);
        }

        public async Task<bool> SaleNumberExistsAsync(string saleNumber)
        {
            return await _context.Sales.AnyAsync(s => s.SaleNumber == saleNumber);
        }

        public async Task<IEnumerable<Sale>> GetAllAsync()
        {
            return await _context.Sales
                .Include(s => s.SaleItems)
                .Include(s => s.Payments)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();
        }
    }
} 
