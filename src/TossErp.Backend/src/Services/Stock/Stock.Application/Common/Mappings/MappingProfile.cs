using AutoMapper;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using TossErp.Stock.Domain.Aggregates.WarehouseAggregate;
using TossErp.Stock.Domain.Aggregates.WarehouseAggregate.Entities;
using TossErp.Stock.Domain.Aggregates.StockEntryAggregate;
using TossErp.Stock.Domain.Aggregates.StockEntryAggregate.Entities;

namespace TossErp.Stock.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Item mappings
        CreateMap<ItemAggregate, ItemDto>()
            .ForMember(dest => dest.ItemCode, opt => opt.MapFrom(src => src.ItemCode.Value))
            .ForMember(dest => dest.StockUOM, opt => opt.MapFrom(src => src.StockUOM.Code))
            .ForMember(dest => dest.ItemType, opt => opt.MapFrom(src => src.ItemType.ToString()))
            .ForMember(dest => dest.ValuationMethod, opt => opt.MapFrom(src => src.ValuationMethod.ToString()))
            .ForMember(dest => dest.ItemStatus, opt => opt.MapFrom(src => src.ItemStatus.ToString()))
            .ForMember(dest => dest.PriorityLevel, opt => opt.MapFrom(src => src.PriorityLevel.ToString()));

        // Warehouse mappings
        CreateMap<WarehouseAggregate, WarehouseDto>()
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code.Value));

        // Bin mappings
        CreateMap<Bin, BinDto>()
            .ForMember(dest => dest.BinCode, opt => opt.MapFrom(src => src.BinCode.Value));

        // StockEntry mappings
        CreateMap<StockEntryAggregate, StockEntryDto>()
            .ForMember(dest => dest.StockEntryNo, opt => opt.MapFrom(src => src.EntryNumber));

        CreateMap<StockEntryDetail, StockEntryDetailDto>();
        CreateMap<StockEntryAdditionalCost, StockEntryAdditionalCostDto>();
    }
} 
