using TossErp.Stock.Domain.Entities;

namespace TossErp.Stock.Application.Common.Models;

public class LookupDto
{
    public Guid Id { get; init; }

    public string? Title { get; init; }

    public string Name { get; init; } = string.Empty;

    private class Mapping : Profile
    {
        public Mapping()
        {
            // Todo mappings removed
        }
    }
}
