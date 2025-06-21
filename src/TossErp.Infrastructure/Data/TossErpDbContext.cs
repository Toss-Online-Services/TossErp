using Microsoft.EntityFrameworkCore;
using TossErp.Domain.AggregatesModel.CooperativeAggregate;
using TossErp.Domain.AggregatesModel.StokvelAggregate;
using TossErp.Domain.AggregatesModel.TownshipEnterpriseAggregate;
using TossErp.Domain.AggregatesModel.GroupPurchaseAggregate;
using TossErp.Domain.AggregatesModel.SaleAggregate;
using TossErp.Domain.AggregatesModel.ProductAggregate;
using TossErp.Domain.SeedWork;

namespace TossErp.Infrastructure.Data
{
    public class TossErpDbContext : DbContext, IUnitOfWork
    {
        public TossErpDbContext(DbContextOptions<TossErpDbContext> options) : base(options)
        {
        }

        public DbSet<Cooperative> Cooperatives { get; set; }
        public DbSet<CooperativeMember> CooperativeMembers { get; set; }
        public DbSet<CooperativeDocument> CooperativeDocuments { get; set; }
        public DbSet<CooperativeMeeting> CooperativeMeetings { get; set; }

        public DbSet<Stokvel> Stokvels { get; set; }
        public DbSet<StokvelMember> StokvelMembers { get; set; }
        public DbSet<StokvelContribution> StokvelContributions { get; set; }
        public DbSet<StokvelPayout> StokvelPayouts { get; set; }
        public DbSet<StokvelMeeting> StokvelMeetings { get; set; }

        public DbSet<TownshipEnterprise> TownshipEnterprises { get; set; }
        public DbSet<BusinessContact> BusinessContacts { get; set; }
        public DbSet<BusinessDocument> BusinessDocuments { get; set; }
        public DbSet<BusinessLicense> BusinessLicenses { get; set; }

        public DbSet<GroupPurchase> GroupPurchases { get; set; }
        public DbSet<GroupPurchaseMember> GroupPurchaseMembers { get; set; }

        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure entity relationships and constraints here
            // This will be expanded as needed
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await SaveChangesAsync(cancellationToken);
            return true;
        }
    }
} 
