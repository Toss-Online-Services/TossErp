using Microsoft.EntityFrameworkCore;
using TossErp.POS.Domain.AggregatesModel.SaleAggregate;

namespace TossErp.POS.Infrastructure.Data
{
    public class POSDbContext : DbContext
    {
        public POSDbContext(DbContextOptions<POSDbContext> options) : base(options)
        {
        }

        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<SalePayment> SalePayments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(POSDbContext).Assembly);
        }
    }
} 
