using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TossErp.HR.Application.Common.Interfaces;

namespace TossErp.HR.Infrastructure.Data;

public class HRDbContext : DbContext, IHRDbContext
{
    public HRDbContext(DbContextOptions<HRDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
