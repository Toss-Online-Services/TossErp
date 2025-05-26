using AutoMapper;
using Catalog.Domain.AggregatesModel.CatalogAggregate;
using Catalog.Domain.DTOs;

namespace Catalog.Domain.Mappings;

public class CatalogMappingProfile : Profile
{
    public CatalogMappingProfile()
    {
        CreateMap<CatalogItem, CatalogItemDto>()
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Price.Currency))
            .ForMember(dest => dest.CatalogType, opt => opt.MapFrom(src => src.CatalogType.Type))
            .ForMember(dest => dest.CatalogBrand, opt => opt.MapFrom(src => src.CatalogBrand.Brand));

        CreateMap<CatalogType, string>().ConvertUsing(src => src.Type);
        CreateMap<CatalogBrand, string>().ConvertUsing(src => src.Brand);
    }
} 
