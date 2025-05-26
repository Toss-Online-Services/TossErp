using AutoMapper;
using Catalog.Application.DTOs;
using Catalog.Domain.Entities;

namespace Catalog.Application.MappingProfiles;

public class CatalogMappingProfile : Profile
{
    public CatalogMappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.ProductCategories, opt => opt.MapFrom(src => src.ProductCategories))
            .ForMember(dest => dest.ProductPictures, opt => opt.MapFrom(src => src.ProductPictures))
            .ForMember(dest => dest.ProductAttributeMappings, opt => opt.MapFrom(src => src.ProductAttributeMappings))
            .ForMember(dest => dest.ProductReviews, opt => opt.MapFrom(src => src.ProductReviews))
            .ForMember(dest => dest.RelatedProducts, opt => opt.MapFrom(src => src.RelatedProducts))
            .ForMember(dest => dest.CrossSellProducts, opt => opt.MapFrom(src => src.CrossSellProducts));

        CreateMap<ProductCategory, ProductCategoryDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));

        CreateMap<ProductPicture, ProductPictureDto>();
        CreateMap<ProductAttributeMapping, ProductAttributeMappingDto>()
            .ForMember(dest => dest.ProductAttributeName, opt => opt.MapFrom(src => src.ProductAttribute != null ? src.ProductAttribute.Name : null))
            .ForMember(dest => dest.ProductAttributeValues, opt => opt.MapFrom(src => src.ProductAttributeValues));
        CreateMap<ProductAttributeValue, ProductAttributeValueDto>();
        CreateMap<ProductReview, ProductReviewDto>();
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.SubCategories))
            .ForMember(dest => dest.ProductCategories, opt => opt.MapFrom(src => src.ProductCategories));
        CreateMap<ProductAttribute, ProductAttributeDto>()
            .ForMember(dest => dest.ProductAttributeMappings, opt => opt.MapFrom(src => src.ProductAttributeMappings));
        CreateMap<ProductTag, ProductTagDto>();
        CreateMap<RelatedProduct, RelatedProductDto>();
        // Add other mappings as needed
    }
} 
