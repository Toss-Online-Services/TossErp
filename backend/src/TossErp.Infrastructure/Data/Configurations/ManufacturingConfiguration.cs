using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Domain.Entities.Manufacturing;

namespace TossErp.Infrastructure.Data.Configurations;

public class BillOfMaterialsConfiguration : IEntityTypeConfiguration<BillOfMaterials>
{
    public void Configure(EntityTypeBuilder<BillOfMaterials> builder)
    {
        builder.ToTable("BillsOfMaterials");
        
        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.BomNumber)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(b => b.BomNumber)
            .IsUnique();
        
        builder.Property(b => b.ProductName)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(b => b.ProductSku)
            .HasMaxLength(50);
        
        builder.Property(b => b.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(b => b.Type)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(b => b.Quantity)
            .HasPrecision(18, 4);
        
        builder.Property(b => b.MaterialCost)
            .HasPrecision(18, 2);
        
        builder.Property(b => b.LaborCost)
            .HasPrecision(18, 2);
        
        builder.Property(b => b.OverheadCost)
            .HasPrecision(18, 2);
        
        builder.Property(b => b.TotalCost)
            .HasPrecision(18, 2);
        
        builder.Property(b => b.EstimatedProductionTime)
            .HasPrecision(10, 2);
        
        builder.Property(b => b.Description)
            .HasMaxLength(1000);
        
        builder.Property(b => b.ApprovedByName)
            .HasMaxLength(200);
        
        builder.HasIndex(b => b.Status);
        builder.HasIndex(b => b.ProductId);
        builder.HasIndex(b => b.Version);
        
        // Relationships
        builder.HasMany(b => b.Items)
            .WithOne(i => i.BillOfMaterials)
            .HasForeignKey(i => i.BomId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(b => b.Operations)
            .WithOne(o => o.BillOfMaterials)
            .HasForeignKey(o => o.BomId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class BomItemConfiguration : IEntityTypeConfiguration<BomItem>
{
    public void Configure(EntityTypeBuilder<BomItem> builder)
    {
        builder.ToTable("BomItems");
        
        builder.HasKey(i => i.Id);
        
        builder.Property(i => i.ComponentName)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(i => i.ComponentSku)
            .HasMaxLength(50);
        
        builder.Property(i => i.Quantity)
            .HasPrecision(18, 4);
        
        builder.Property(i => i.WastagePercentage)
            .HasPrecision(5, 2);
        
        builder.Property(i => i.UnitCost)
            .HasPrecision(18, 2);
        
        builder.Property(i => i.SupplyType)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(i => i.PreferredSupplierName)
            .HasMaxLength(200);
        
        builder.Property(i => i.LeadTimeDays)
            .HasPrecision(10, 2);
        
        builder.Property(i => i.ReferenceDesignator)
            .HasMaxLength(50);
        
        builder.HasIndex(i => i.BomId);
        builder.HasIndex(i => i.ComponentId);
        builder.HasIndex(i => i.Sequence);
        
        // Computed property - don't store
        builder.Ignore(i => i.EffectiveQuantity);
        builder.Ignore(i => i.TotalCost);
    }
}

public class BomOperationConfiguration : IEntityTypeConfiguration<BomOperation>
{
    public void Configure(EntityTypeBuilder<BomOperation> builder)
    {
        builder.ToTable("BomOperations");
        
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.OperationCode)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(o => o.OperationName)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(o => o.Type)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(o => o.WorkCenterName)
            .HasMaxLength(200);
        
        builder.Property(o => o.MachineName)
            .HasMaxLength(200);
        
        builder.Property(o => o.SetupTime)
            .HasPrecision(10, 2);
        
        builder.Property(o => o.RunTimePerUnit)
            .HasPrecision(10, 4);
        
        builder.Property(o => o.WaitTime)
            .HasPrecision(10, 2);
        
        builder.Property(o => o.MoveTime)
            .HasPrecision(10, 2);
        
        builder.Property(o => o.TotalTime)
            .HasPrecision(10, 2);
        
        builder.Property(o => o.LaborRate)
            .HasPrecision(18, 2);
        
        builder.Property(o => o.LaborCost)
            .HasPrecision(18, 2);
        
        builder.Property(o => o.MachineRate)
            .HasPrecision(18, 2);
        
        builder.Property(o => o.MachineCost)
            .HasPrecision(18, 2);
        
        builder.Property(o => o.OverheadRate)
            .HasPrecision(18, 2);
        
        builder.Property(o => o.OverheadCost)
            .HasPrecision(18, 2);
        
        builder.HasIndex(o => o.BomId);
        builder.HasIndex(o => o.Sequence);
        
        // Computed property
        builder.Ignore(o => o.TotalCost);
    }
}

public class WorkOrderConfiguration : IEntityTypeConfiguration<WorkOrder>
{
    public void Configure(EntityTypeBuilder<WorkOrder> builder)
    {
        builder.ToTable("WorkOrders");
        
        builder.HasKey(w => w.Id);
        
        builder.Property(w => w.WorkOrderNumber)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(w => w.WorkOrderNumber)
            .IsUnique();
        
        builder.Property(w => w.Type)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(w => w.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(w => w.ProductName)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(w => w.ProductSku)
            .HasMaxLength(50);
        
        builder.Property(w => w.BomNumber)
            .HasMaxLength(50);
        
        builder.Property(w => w.QuantityOrdered)
            .HasPrecision(18, 4);
        
        builder.Property(w => w.QuantityProduced)
            .HasPrecision(18, 4);
        
        builder.Property(w => w.QuantityRejected)
            .HasPrecision(18, 4);
        
        builder.Property(w => w.WarehouseName)
            .HasMaxLength(200);
        
        builder.Property(w => w.WorkCenterName)
            .HasMaxLength(200);
        
        builder.Property(w => w.EstimatedMaterialCost)
            .HasPrecision(18, 2);
        
        builder.Property(w => w.EstimatedLaborCost)
            .HasPrecision(18, 2);
        
        builder.Property(w => w.EstimatedOverheadCost)
            .HasPrecision(18, 2);
        
        builder.Property(w => w.EstimatedTotalCost)
            .HasPrecision(18, 2);
        
        builder.Property(w => w.ActualMaterialCost)
            .HasPrecision(18, 2);
        
        builder.Property(w => w.ActualLaborCost)
            .HasPrecision(18, 2);
        
        builder.Property(w => w.ActualOverheadCost)
            .HasPrecision(18, 2);
        
        builder.Property(w => w.ActualTotalCost)
            .HasPrecision(18, 2);
        
        builder.Property(w => w.SalesOrderNumber)
            .HasMaxLength(50);
        
        builder.Property(w => w.CustomerName)
            .HasMaxLength(200);
        
        builder.Property(w => w.ApprovedByName)
            .HasMaxLength(200);
        
        builder.HasIndex(w => w.Status);
        builder.HasIndex(w => w.PlannedStartDate);
        builder.HasIndex(w => w.ProductId);
        builder.HasIndex(w => w.BomId);
        
        // Computed property
        builder.Ignore(w => w.QuantityRemaining);
        
        // Relationships
        builder.HasMany(w => w.Operations)
            .WithOne(o => o.WorkOrder)
            .HasForeignKey(o => o.WorkOrderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(w => w.Materials)
            .WithOne(m => m.WorkOrder)
            .HasForeignKey(m => m.WorkOrderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(w => w.ProductionEntries)
            .WithOne(p => p.WorkOrder)
            .HasForeignKey(p => p.WorkOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class WorkOrderOperationConfiguration : IEntityTypeConfiguration<WorkOrderOperation>
{
    public void Configure(EntityTypeBuilder<WorkOrderOperation> builder)
    {
        builder.ToTable("WorkOrderOperations");
        
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.OperationCode)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(o => o.OperationName)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(o => o.WorkCenterName)
            .HasMaxLength(200);
        
        builder.Property(o => o.MachineName)
            .HasMaxLength(200);
        
        builder.Property(o => o.OperatorName)
            .HasMaxLength(200);
        
        builder.Property(o => o.EstimatedSetupTime)
            .HasPrecision(10, 2);
        
        builder.Property(o => o.EstimatedRunTime)
            .HasPrecision(10, 2);
        
        builder.Property(o => o.ActualSetupTime)
            .HasPrecision(10, 2);
        
        builder.Property(o => o.ActualRunTime)
            .HasPrecision(10, 2);
        
        builder.Property(o => o.QuantityCompleted)
            .HasPrecision(18, 4);
        
        builder.Property(o => o.QuantityRejected)
            .HasPrecision(18, 4);
        
        builder.Property(o => o.QuantityScrapped)
            .HasPrecision(18, 4);
        
        builder.Property(o => o.EstimatedCost)
            .HasPrecision(18, 2);
        
        builder.Property(o => o.ActualCost)
            .HasPrecision(18, 2);
        
        builder.Property(o => o.InspectedByName)
            .HasMaxLength(200);
        
        builder.HasIndex(o => o.WorkOrderId);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.Sequence);
    }
}

public class WorkOrderMaterialConfiguration : IEntityTypeConfiguration<WorkOrderMaterial>
{
    public void Configure(EntityTypeBuilder<WorkOrderMaterial> builder)
    {
        builder.ToTable("WorkOrderMaterials");
        
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.ComponentName)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(m => m.ComponentSku)
            .HasMaxLength(50);
        
        builder.Property(m => m.QuantityRequired)
            .HasPrecision(18, 4);
        
        builder.Property(m => m.QuantityIssued)
            .HasPrecision(18, 4);
        
        builder.Property(m => m.QuantityConsumed)
            .HasPrecision(18, 4);
        
        builder.Property(m => m.QuantityReturned)
            .HasPrecision(18, 4);
        
        builder.Property(m => m.QuantityScrapped)
            .HasPrecision(18, 4);
        
        builder.Property(m => m.UnitCost)
            .HasPrecision(18, 2);
        
        builder.Property(m => m.WarehouseName)
            .HasMaxLength(200);
        
        builder.Property(m => m.BinLocation)
            .HasMaxLength(100);
        
        builder.Property(m => m.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(m => m.IssuedByName)
            .HasMaxLength(200);
        
        builder.Property(m => m.BatchNumber)
            .HasMaxLength(50);
        
        builder.Property(m => m.SerialNumber)
            .HasMaxLength(50);
        
        builder.HasIndex(m => m.WorkOrderId);
        builder.HasIndex(m => m.ComponentId);
        builder.HasIndex(m => m.Status);
        
        // Computed property
        builder.Ignore(m => m.TotalCost);
    }
}

public class ProductionEntryConfiguration : IEntityTypeConfiguration<ProductionEntry>
{
    public void Configure(EntityTypeBuilder<ProductionEntry> builder)
    {
        builder.ToTable("ProductionEntries");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.EntryNumber)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(p => p.EntryNumber)
            .IsUnique();
        
        builder.Property(p => p.QuantityProduced)
            .HasPrecision(18, 4);
        
        builder.Property(p => p.QuantityRejected)
            .HasPrecision(18, 4);
        
        builder.Property(p => p.QuantityScrapped)
            .HasPrecision(18, 4);
        
        builder.Property(p => p.ActualHours)
            .HasPrecision(10, 2);
        
        builder.Property(p => p.WorkCenterName)
            .HasMaxLength(200);
        
        builder.Property(p => p.OperatorName)
            .HasMaxLength(200);
        
        builder.Property(p => p.SupervisorName)
            .HasMaxLength(200);
        
        builder.Property(p => p.InspectedByName)
            .HasMaxLength(200);
        
        builder.Property(p => p.MaterialCost)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.LaborCost)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.OverheadCost)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.TotalCost)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.PostedByName)
            .HasMaxLength(200);
        
        builder.HasIndex(p => p.WorkOrderId);
        builder.HasIndex(p => p.ProductionDate);
        builder.HasIndex(p => p.IsPosted);
    }
}

