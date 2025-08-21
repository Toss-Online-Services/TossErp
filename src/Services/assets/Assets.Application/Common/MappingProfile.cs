using AutoMapper;

namespace TossErp.Assets.Application.Common;

/// <summary>
/// AutoMapper profile for Asset mappings
/// </summary>
public class AssetMappingProfile : Profile
{
    public AssetMappingProfile()
    {
        CreateMap<Asset, AssetDto>()
            .ForMember(dest => dest.AssetNumber, opt => opt.MapFrom(src => src.AssetNumber.Value))
            .ForMember(dest => dest.PurchasePrice, opt => opt.MapFrom(src => src.PurchasePrice != null ? src.PurchasePrice.Amount : (decimal?)null))
            .ForMember(dest => dest.PurchaseCurrency, opt => opt.MapFrom(src => src.PurchasePrice != null ? src.PurchasePrice.Currency.ToString() : null))
            .ForMember(dest => dest.CurrentValue, opt => opt.MapFrom(src => src.CurrentValue != null ? src.CurrentValue.Amount : (decimal?)null))
            .ForMember(dest => dest.WarrantyStartDate, opt => opt.MapFrom(src => src.WarrantyPeriod != null ? src.WarrantyPeriod.StartDate : (DateOnly?)null))
            .ForMember(dest => dest.WarrantyEndDate, opt => opt.MapFrom(src => src.WarrantyPeriod != null ? src.WarrantyPeriod.EndDate : (DateOnly?)null))
            .ForMember(dest => dest.WarrantyPeriodMonths, opt => opt.MapFrom(src => src.WarrantyPeriod != null ? src.WarrantyPeriod.DurationInMonths : (int?)null))
            .ForMember(dest => dest.MaintenanceIntervalDays, opt => opt.MapFrom(src => src.MaintenanceSchedule.IntervalDays))
            .ForMember(dest => dest.DepreciationMethod, opt => opt.MapFrom(src => src.DepreciationInfo != null ? src.DepreciationInfo.Method : null))
            .ForMember(dest => dest.UsefulLifeYears, opt => opt.MapFrom(src => src.DepreciationInfo != null ? src.DepreciationInfo.UsefulLifeYears : (int?)null))
            .ForMember(dest => dest.SalvageValue, opt => opt.MapFrom(src => src.DepreciationInfo != null && src.DepreciationInfo.SalvageValue != null ? src.DepreciationInfo.SalvageValue.Amount : (decimal?)null))
            .ForMember(dest => dest.AccumulatedDepreciation, opt => opt.MapFrom(src => src.AccumulatedDepreciation != null ? src.AccumulatedDepreciation.Amount : (decimal?)null));

        CreateMap<Asset, AssetSummaryDto>()
            .ForMember(dest => dest.AssetNumber, opt => opt.MapFrom(src => src.AssetNumber.Value))
            .ForMember(dest => dest.PurchasePrice, opt => opt.MapFrom(src => src.PurchasePrice != null ? src.PurchasePrice.Amount : (decimal?)null))
            .ForMember(dest => dest.PurchaseCurrency, opt => opt.MapFrom(src => src.PurchasePrice != null ? src.PurchasePrice.Currency.ToString() : null))
            .ForMember(dest => dest.CurrentValue, opt => opt.MapFrom(src => src.CurrentValue != null ? src.CurrentValue.Amount : (decimal?)null));

        CreateMap<MaintenanceRecord, MaintenanceRecordDto>()
            .ForMember(dest => dest.EstimatedCost, opt => opt.MapFrom(src => src.EstimatedCost != null ? src.EstimatedCost.Amount : (decimal?)null))
            .ForMember(dest => dest.ActualCost, opt => opt.MapFrom(src => src.ActualCost != null ? src.ActualCost.Amount : (decimal?)null))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.EstimatedCost != null ? src.EstimatedCost.Currency.ToString() : (src.ActualCost != null ? src.ActualCost.Currency.ToString() : null)))
            .ForMember(dest => dest.AssetName, opt => opt.Ignore())
            .ForMember(dest => dest.AssetNumber, opt => opt.Ignore());

        CreateMap<AssetTransfer, AssetTransferDto>()
            .ForMember(dest => dest.AssetName, opt => opt.Ignore())
            .ForMember(dest => dest.AssetNumber, opt => opt.Ignore());

        CreateMap<AssetDisposal, AssetDisposalDto>()
            .ForMember(dest => dest.DisposalValue, opt => opt.MapFrom(src => src.DisposalValue != null ? src.DisposalValue.Amount : (decimal?)null))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.DisposalValue != null ? src.DisposalValue.Currency.ToString() : null))
            .ForMember(dest => dest.AssetName, opt => opt.Ignore())
            .ForMember(dest => dest.AssetNumber, opt => opt.Ignore())
            .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => src.Documents));

        CreateMap<AssetValuation, AssetValuationDto>()
            .ForMember(dest => dest.CurrentValue, opt => opt.MapFrom(src => src.CurrentValue.Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.CurrentValue.Currency.ToString()))
            .ForMember(dest => dest.AssetName, opt => opt.Ignore())
            .ForMember(dest => dest.AssetNumber, opt => opt.Ignore())
            .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => src.Documents));

        CreateMap<AssetDocument, AssetDocumentDto>();

        CreateMap<Location, LocationDto>();
    }
}

/// <summary>
/// Extension methods for mapping Asset entities to DTOs
/// </summary>
public static class AssetMappingExtensions
{
    /// <summary>
    /// Maps an Asset entity to AssetDto
    /// </summary>
    public static AssetDto ToDto(this Asset asset)
    {
        return new AssetDto
        {
            Id = asset.Id,
            TenantId = asset.TenantId,
            AssetNumber = asset.AssetNumber.Value,
            Name = asset.Name,
            Description = asset.Description,
            Category = asset.Category,
            Type = asset.Type,
            Status = asset.Status,
            SerialNumber = asset.SerialNumber,
            Model = asset.Model,
            Manufacturer = asset.Manufacturer,
            PurchaseDate = asset.PurchaseDate,
            PurchasePrice = asset.PurchasePrice?.Amount,
            PurchaseCurrency = asset.PurchasePrice?.Currency.ToString(),
            CurrentValue = asset.CurrentValue?.Amount,
            WarrantyStartDate = asset.WarrantyPeriod?.StartDate,
            WarrantyEndDate = asset.WarrantyPeriod?.EndDate,
            WarrantyPeriodMonths = asset.WarrantyPeriod?.DurationInMonths,
            WarrantyProvider = asset.WarrantyProvider,
            LocationId = asset.LocationId,
            DepartmentId = asset.DepartmentId,
            AssignedToUserId = asset.AssignedToUserId,
            LastMaintenanceDate = asset.LastMaintenanceDate,
            NextMaintenanceDate = asset.NextMaintenanceDate,
            MaintenanceIntervalDays = asset.MaintenanceSchedule.IntervalDays,
            DepreciationMethod = asset.DepreciationInfo?.Method,
            UsefulLifeYears = asset.DepreciationInfo?.UsefulLifeYears,
            SalvageValue = asset.DepreciationInfo?.SalvageValue?.Amount,
            AccumulatedDepreciation = asset.AccumulatedDepreciation?.Amount,
            CreatedAt = asset.CreatedAt,
            CreatedBy = asset.CreatedBy,
            LastModified = asset.LastModified,
            LastModifiedBy = asset.LastModifiedBy
        };
    }

    /// <summary>
    /// Maps an Asset entity to AssetSummaryDto
    /// </summary>
    public static AssetSummaryDto ToSummaryDto(this Asset asset)
    {
        return new AssetSummaryDto
        {
            Id = asset.Id,
            AssetNumber = asset.AssetNumber.Value,
            Name = asset.Name,
            Category = asset.Category,
            Type = asset.Type,
            Status = asset.Status,
            SerialNumber = asset.SerialNumber,
            Manufacturer = asset.Manufacturer,
            Model = asset.Model,
            PurchaseDate = asset.PurchaseDate,
            PurchasePrice = asset.PurchasePrice?.Amount,
            PurchaseCurrency = asset.PurchasePrice?.Currency.ToString(),
            CurrentValue = asset.CurrentValue?.Amount,
            LocationId = asset.LocationId,
            DepartmentId = asset.DepartmentId,
            AssignedToUserId = asset.AssignedToUserId,
            LastMaintenanceDate = asset.LastMaintenanceDate,
            NextMaintenanceDate = asset.NextMaintenanceDate,
            CreatedAt = asset.CreatedAt,
            LastModified = asset.LastModified
        };
    }

    /// <summary>
    /// Maps a MaintenanceRecord entity to MaintenanceRecordDto with Asset information
    /// </summary>
    public static MaintenanceRecordDto ToDto(this MaintenanceRecord record, Asset asset)
    {
        return new MaintenanceRecordDto
        {
            Id = record.Id,
            AssetId = record.AssetId,
            AssetName = asset.Name,
            AssetNumber = asset.AssetNumber.Value,
            Type = record.Type,
            Description = record.Description,
            Status = record.Status,
            ScheduledDate = record.ScheduledDate,
            DueDate = record.DueDate,
            StartDate = record.StartDate,
            CompletionDate = record.CompletionDate,
            EstimatedCost = record.EstimatedCost?.Amount,
            ActualCost = record.ActualCost?.Amount,
            Currency = record.EstimatedCost?.Currency.ToString() ?? record.ActualCost?.Currency.ToString(),
            EstimatedDurationHours = record.EstimatedDurationHours,
            ActualDurationHours = record.ActualDurationHours,
            AssignedTechnician = record.AssignedTechnician,
            Instructions = record.Instructions,
            WorkPerformed = record.WorkPerformed,
            PartsUsed = record.PartsUsed,
            Priority = record.Priority,
            ScheduledBy = record.ScheduledBy,
            ScheduledAt = record.ScheduledAt,
            CompletedBy = record.CompletedBy
        };
    }

    /// <summary>
    /// Maps an AssetTransfer entity to AssetTransferDto with Asset information
    /// </summary>
    public static AssetTransferDto ToDto(this AssetTransfer transfer, Asset asset)
    {
        return new AssetTransferDto
        {
            Id = transfer.Id,
            AssetId = transfer.AssetId,
            AssetName = asset.Name,
            AssetNumber = asset.AssetNumber.Value,
            FromLocationId = transfer.FromLocationId,
            ToLocationId = transfer.ToLocationId,
            FromDepartmentId = transfer.FromDepartmentId,
            ToDepartmentId = transfer.ToDepartmentId,
            FromUserId = transfer.FromUserId,
            ToUserId = transfer.ToUserId,
            TransferDate = transfer.TransferDate,
            Reason = transfer.Reason,
            Notes = transfer.Notes,
            TransferredBy = transfer.TransferredBy,
            TransferredAt = transfer.TransferredAt
        };
    }

    /// <summary>
    /// Maps an AssetDisposal entity to AssetDisposalDto with Asset information
    /// </summary>
    public static AssetDisposalDto ToDto(this AssetDisposal disposal, Asset asset)
    {
        return new AssetDisposalDto
        {
            Id = disposal.Id,
            AssetId = disposal.AssetId,
            AssetName = asset.Name,
            AssetNumber = asset.AssetNumber.Value,
            Method = disposal.Method,
            DisposalDate = disposal.DisposalDate,
            Reason = disposal.Reason,
            DisposalValue = disposal.DisposalValue?.Amount,
            Currency = disposal.DisposalValue?.Currency.ToString(),
            DisposedTo = disposal.DisposedTo,
            AuthorizedBy = disposal.AuthorizedBy,
            Notes = disposal.Notes,
            DisposedBy = disposal.DisposedBy,
            DisposedAt = disposal.DisposedAt,
            Documents = disposal.Documents.Select(d => d.ToDto()).ToList()
        };
    }

    /// <summary>
    /// Maps an AssetValuation entity to AssetValuationDto with Asset information
    /// </summary>
    public static AssetValuationDto ToDto(this AssetValuation valuation, Asset asset)
    {
        return new AssetValuationDto
        {
            Id = valuation.Id,
            AssetId = valuation.AssetId,
            AssetName = asset.Name,
            AssetNumber = asset.AssetNumber.Value,
            CurrentValue = valuation.CurrentValue.Amount,
            Currency = valuation.CurrentValue.Currency.ToString(),
            ValuationDate = valuation.ValuationDate,
            ValuationMethod = valuation.ValuationMethod,
            ValuedBy = valuation.ValuedBy,
            Notes = valuation.Notes,
            RecordedBy = valuation.RecordedBy,
            RecordedAt = valuation.RecordedAt,
            Documents = valuation.Documents.Select(d => d.ToDto()).ToList()
        };
    }

    /// <summary>
    /// Maps an AssetDocument entity to AssetDocumentDto
    /// </summary>
    public static AssetDocumentDto ToDto(this AssetDocument document)
    {
        return new AssetDocumentDto
        {
            Id = document.Id,
            AssetId = document.AssetId,
            FileName = document.FileName,
            FilePath = document.FilePath,
            FileSize = document.FileSize,
            ContentType = document.ContentType,
            DocumentType = document.DocumentType,
            Description = document.Description,
            UploadedBy = document.UploadedBy,
            UploadedAt = document.UploadedAt
        };
    }

    /// <summary>
    /// Maps a Location entity to LocationDto
    /// </summary>
    public static LocationDto ToDto(this Location location)
    {
        return new LocationDto
        {
            Id = location.Id,
            Name = location.Name,
            Description = location.Description,
            Address = location.Address,
            City = location.City,
            State = location.State,
            PostalCode = location.PostalCode,
            Country = location.Country,
            IsActive = location.IsActive,
            CreatedAt = location.CreatedAt,
            LastModified = location.LastModified
        };
    }
}
