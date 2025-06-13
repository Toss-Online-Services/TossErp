using Microsoft.EntityFrameworkCore;
using eShop.POS.Domain.AggregatesModel.BuyerAggregate;
using eShop.POS.Domain.Repositories;
using eShop.POS.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShop.POS.Infrastructure.Repositories;

public class BuyerRepository : IBuyerRepository
{
    private readonly POSContext _context;

    public BuyerRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Buyer?> GetByIdAsync(string id)
    {
        return await _context.Buyers
            .Include(b => b.PaymentMethods)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Buyer>> GetAllAsync()
    {
        return await _context.Buyers
            .Include(b => b.PaymentMethods)
            .ToListAsync();
    }

    public async Task AddAsync(Buyer buyer)
    {
        await _context.Buyers.AddAsync(buyer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Buyer buyer)
    {
        _context.Entry(buyer).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Buyer buyer)
    {
        _context.Buyers.Remove(buyer);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await _context.Buyers.AnyAsync(b => b.Id == id);
    }

    public async Task<Buyer?> GetByIdentityGuidAsync(string identityGuid)
    {
        return await _context.Buyers
            .Include(b => b.PaymentMethods)
            .FirstOrDefaultAsync(b => b.IdentityGuid == identityGuid);
    }
}
