using Microsoft.AspNetCore.Identity;
using Toss.Domain.Entities.Businesses;

namespace Toss.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public ICollection<UserBusiness> Businesses { get; set; } = new List<UserBusiness>();
}
