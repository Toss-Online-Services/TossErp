using Toss.Domain.Entities;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Entities.Vendors;

namespace Toss.Application.Common.Models;

public class LookupDto
{
    public int Id { get; init; }

    public string? Title { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Store, LookupDto>()
                .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Name));
            CreateMap<Product, LookupDto>()
                .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Name));
            CreateMap<Vendor, LookupDto>()
                .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Name));
        }
    }
}
