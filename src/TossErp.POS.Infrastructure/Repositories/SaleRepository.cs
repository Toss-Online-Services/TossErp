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

        public IUnitOfWork UnitOfWork => _context;

        public async Task<Sale?> GetByIdAsync(Guid id)
        {
            return await _context.Sales
                .Include(s => s.SaleItems)
                .Include(s => s.Payments)
                .FirstOrDefaultAsync(s => s.Id == id);
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

        public async Task AddAsync(Sale sale)
        {
            await _context.Sales.AddAsync(sale);
        }

        public void Update(Sale sale)
        {
            _context.Entry(sale).State = EntityState.Modified;
        }

        public void Delete(Sale sale)
        {
            _context.Sales.Remove(sale);
        }
    }
} 
