using Toss.Domain.Entities;
using Toss.Domain.Entities.Inventory;
using Toss.Domain.Entities.Suppliers;

namespace Toss.Application.Common.Models;

public class LookupDto
{
    public int Id { get; init; }

    public string? Title { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Shop, LookupDto>()
                .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Name));
            CreateMap<Product, LookupDto>()
                .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Name));
            CreateMap<Supplier, LookupDto>()
                .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Name));
        }
    }
}
