using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Toss.Domain.Entities.Directory;
using Toss.Domain.Entities.Tax;
using Toss.Infrastructure.Data;
using Toss.Infrastructure.Identity;
using Toss.Infrastructure.Services.Tenancy;

namespace Toss.Infrastructure.IntegrationTests;

[TestFixture]
public class ReferenceDataSeederTests
{
    [Test]
    public async Task SeedReferenceData_Inserts_ZarCurrency_AndVatRate()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"ReferenceData-{Guid.NewGuid()}")
            .Options;

        await using var context = new ApplicationDbContext(options, new NullBusinessContext());

        var initializer = new ApplicationDbContextInitialiser(
            NullLogger<ApplicationDbContextInitialiser>.Instance,
            context,
            CreateUserManagerMock().Object,
            CreateRoleManagerMock().Object);

        var method = typeof(ApplicationDbContextInitialiser)
            .GetMethod("SeedReferenceDataAsync", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.That(method, Is.Not.Null, "SeedReferenceDataAsync should exist on the initializer.");

        if (method?.Invoke(initializer, null) is Task task)
        {
            await task;
        }

        var zarCurrency = await context.Set<Currency>()
            .SingleOrDefaultAsync(c => c.CurrencyCode == "ZAR");

        Assert.That(zarCurrency, Is.Not.Null, "ZAR currency should be seeded.");

        var vatRate = await context.Set<TaxRate>()
            .SingleOrDefaultAsync(r => r.Name == "VAT 15%");

        Assert.That(vatRate, Is.Not.Null, "VAT 15% tax rate should be seeded.");
        Assert.That(vatRate!.Percentage, Is.EqualTo(15m));
    }

    private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        return new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }

    private static Mock<RoleManager<IdentityRole>> CreateRoleManagerMock()
    {
        var store = new Mock<IRoleStore<IdentityRole>>();
        return new Mock<RoleManager<IdentityRole>>(store.Object, null!, null!, null!, null!);
    }
}

