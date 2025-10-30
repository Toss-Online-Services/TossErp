using Toss.Domain.Constants;
using Toss.Domain.Entities;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Entities.CRM;
using Toss.Domain.Entities.Vendors;
using Toss.Domain.Entities.Logistics;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Entities.Orders;
using Toss.Domain.Entities.Payments;
using Toss.Domain.Enums;
using Toss.Domain.ValueObjects;
using Toss.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Bogus;

namespace Toss.Infrastructure.Data;

// Extension method removed - seeding now done directly in Program.cs following Microsoft's best practices

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    /// <summary>
    /// Seeds the database with initial data. Called from Program.cs after migrations are applied.
    /// Follows Microsoft's best practices: https://learn.microsoft.com/en-us/dotnet/aspire/database/seed-database-data
    /// </summary>

    public async Task SeedAsync()
    {
        try
        {
            // Check if Identity tables exist before attempting to seed
            if (!await CheckIdentityTablesExistAsync())
            {
                _logger.LogWarning("Identity tables do not exist. Skipping seed data.");
                return;
            }

            _logger.LogInformation("Starting database seeding with comprehensive test data...");

            // Seed roles and users first
            await SeedRolesAndUsersAsync();

            // Seed domain data
            await SeedStoresAsync();
            await SeedProductCategoriesAsync();
            await SeedProductsAsync();
            await SeedVendorsAsync();
            await SeedCustomersAsync();
            await SeedDriversAsync();
            await SeedPurchaseOrdersAsync();
            await SeedSalesAsync();
            await SeedOrdersAsync();
            await SeedPaymentsAsync();

            _logger.LogInformation("✅ Database seeding completed successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ An error occurred while seeding the database.");
            _logger.LogWarning("Database seeding failed, but application will continue. " +
                "Some features may not work correctly without seed data.");
            // Don't throw - allow app to start even if seeding fails
        }
    }

    private async Task SeedRolesAndUsersAsync()
    {
        // Create roles
        var roles = new[] { Roles.Administrator, "StoreOwner", "Vendor", "Driver", "Customer" };
        
        foreach (var roleName in roles)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
                _logger.LogInformation("✅ Created {Role} role.", roleName);
            }
        }

        // Create default administrator
        var adminEmail = "administrator@localhost";
        if (_userManager.Users.All(u => u.UserName != adminEmail))
        {
            var administrator = new ApplicationUser 
            { 
                UserName = adminEmail, 
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(administrator, "Administrator1!");
            await _userManager.AddToRoleAsync(administrator, Roles.Administrator);
            _logger.LogInformation("✅ Created default administrator user: {Email}", adminEmail);
        }

        // Check if we already have enough store owners
        var storeOwnerCount = (await _userManager.GetUsersInRoleAsync("StoreOwner")).Count;
        if (storeOwnerCount >= 15)
        {
            _logger.LogInformation("✅ Store owners already seeded ({Count} existing).", storeOwnerCount);
            return;
        }

        // Create sample store owners (15 total)
        var storeOwnerFaker = new Faker<ApplicationUser>()
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName).ToLower())
            .RuleFor(u => u.UserName, (f, u) => u.Email)
            .RuleFor(u => u.EmailConfirmed, true);

        var usersCreated = 0;
        for (int i = 0; i < 15; i++)
        {
            var user = storeOwnerFaker.Generate();
            // Ensure unique email
            user.Email = $"storeowner{i+1}@toss.local";
            user.UserName = user.Email;
            
            if (_userManager.Users.All(u => u.Email != user.Email))
            {
                var result = await _userManager.CreateAsync(user, "StoreOwner1!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "StoreOwner");
                    usersCreated++;
                }
            }
        }
        
        _logger.LogInformation("✅ Created {Count} store owner users.", usersCreated);
    }

    private async Task SeedStoresAsync()
    {
        // Check if we already have enough stores
        var existingCount = await _context.Stores.CountAsync();
        if (existingCount >= 20)
        {
            _logger.LogInformation("✅ Stores already seeded ({Count} existing).", existingCount);
            return;
        }

        var users = await _userManager.GetUsersInRoleAsync("StoreOwner");
        if (!users.Any())
        {
            _logger.LogWarning("⚠️  No store owners found. Skipping store seeding.");
            return;
        }

        var townshipAreas = new[] 
        { 
            "Soweto", "Alexandra", "Diepsloot", "Tembisa", "Khayelitsha", 
            "Mitchells Plain", "Gugulethu", "Nyanga", "Mamelodi", "Atteridgeville" 
        };

        var storeFaker = new Faker<Store>()
            .RuleFor(s => s.Name, f => $"{f.Company.CompanyName()} {f.PickRandom("Spaza", "Store", "Shop", "Market")}")
            .RuleFor(s => s.Description, f => f.Company.CatchPhrase())
            .RuleFor(s => s.OwnerId, f => f.PickRandom(users).Id)
            .RuleFor(s => s.AreaGroup, f => f.PickRandom(townshipAreas))
            .RuleFor(s => s.Email, f => f.Internet.Email())
            .RuleFor(s => s.ContactPhone, f => new PhoneNumber($"+27{f.Random.Int(6, 8)}{f.Random.Int(10000000, 99999999)}"))
            .RuleFor(s => s.OpeningTime, f => TimeOnly.FromTimeSpan(TimeSpan.FromHours(f.Random.Int(6, 8))))
            .RuleFor(s => s.ClosingTime, f => TimeOnly.FromTimeSpan(TimeSpan.FromHours(f.Random.Int(18, 22))))
            .RuleFor(s => s.IsActive, true)
            .RuleFor(s => s.Currency, "ZAR")
            .RuleFor(s => s.TaxRate, 15m)
            .RuleFor(s => s.Language, "en")
            .RuleFor(s => s.Timezone, "Africa/Johannesburg")
            .RuleFor(s => s.WhatsAppAlertsEnabled, f => f.Random.Bool(0.8f))
            .RuleFor(s => s.GroupBuyingEnabled, f => f.Random.Bool(0.7f))
            .RuleFor(s => s.AIAssistantEnabled, f => f.Random.Bool(0.6f))
            .RuleFor(s => s.CompanyName, (f, s) => s.Name)
            .RuleFor(s => s.CompanyVat, f => f.Random.Replace("4##########"));

        var storesToCreate = Math.Max(0, 20 - existingCount);
        if (storesToCreate == 0)
        {
            _logger.LogInformation("✅ Target store count reached ({Count} stores).", existingCount);
            return;
        }

        var stores = storeFaker.Generate(storesToCreate);
        var faker = new Faker();
        
        // Create addresses and locations for stores  
        foreach (var store in stores)
        {
            var townshipArea = faker.PickRandom(townshipAreas);
            var latitude = faker.Random.Double(-26.3, -25.7);  // Johannesburg latitude
            var longitude = faker.Random.Double(27.8, 28.2);   // Johannesburg longitude
            var zone = faker.PickRandom("North", "South", "East", "West", "Central");
            
            // Set Location (owned entity) - must be created separately for each owner
            store.Location = new Location(latitude, longitude, townshipArea, zone);
            
            // Set Address (owned entity) - create a NEW Location instance for Coordinates
            // Owned entities cannot be shared between different owners
            store.Address = new Address
            {
                Street = new Faker().Address.StreetAddress(),
                City = store.AreaGroup ?? "Johannesburg",
                StateProvince = "Gauteng",
                PostalCode = new Faker().Address.ZipCode(),
                Country = "ZA",
                Coordinates = new Location(latitude, longitude, townshipArea, zone) // NEW instance
            };
        }

        _context.Stores.AddRange(stores);
        await _context.SaveChangesAsync();
        _logger.LogInformation("✅ Seeded {Count} stores (Total: {Total}).", stores.Count, existingCount + stores.Count);
    }

    private async Task SeedProductCategoriesAsync()
    {
        var existingCount = await _context.Set<ProductCategory>().CountAsync();
        if (existingCount >= 10)
        {
            _logger.LogInformation("✅ Product categories already seeded ({Count} existing).", existingCount);
            return;
        }

        var categories = new List<ProductCategory>
        {
            new() { Name = "Groceries", Description = "Essential food and household items", IsActive = true },
            new() { Name = "Fresh Produce", Description = "Fresh fruits and vegetables", IsActive = true },
            new() { Name = "Beverages", Description = "Drinks and refreshments", IsActive = true },
            new() { Name = "Snacks", Description = "Chips, sweets, and quick snacks", IsActive = true },
            new() { Name = "Dairy", Description = "Milk, cheese, and dairy products", IsActive = true },
            new() { Name = "Bakery", Description = "Bread and baked goods", IsActive = true },
            new() { Name = "Household", Description = "Cleaning and household supplies", IsActive = true },
            new() { Name = "Personal Care", Description = "Toiletries and personal hygiene", IsActive = true },
            new() { Name = "Airtime & Data", Description = "Mobile airtime and data bundles", IsActive = true },
            new() { Name = "Frozen Foods", Description = "Frozen meals and products", IsActive = true }
        };

        _context.Set<ProductCategory>().AddRange(categories);
        await _context.SaveChangesAsync();
        _logger.LogInformation("✅ Seeded {Count} product categories.", categories.Count);
    }

    private async Task SeedProductsAsync()
    {
        var existingCount = await _context.Set<Product>().CountAsync();
        if (existingCount >= 27) // We have 27 product names defined
        {
            _logger.LogInformation("✅ Products already seeded ({Count} existing).", existingCount);
            return;
        }

        var categories = await _context.Set<ProductCategory>().ToListAsync();
        if (!categories.Any())
        {
            _logger.LogWarning("⚠️  No product categories found. Skipping product seeding.");
            return;
        }

        var stores = await _context.Stores.ToListAsync();
        if (!stores.Any())
        {
            _logger.LogWarning("⚠️  No stores found. Skipping product seeding.");
            return;
        }

        var productNames = new Dictionary<string, (decimal min, decimal max, string unit)>
        {
            // Groceries
            ["Rice 2kg"] = (35, 60, "bag"),
            ["Maize Meal 5kg"] = (50, 80, "bag"),
            ["Sugar 2.5kg"] = (30, 45, "bag"),
            ["Cooking Oil 750ml"] = (25, 40, "bottle"),
            ["Salt 500g"] = (8, 12, "packet"),
            ["Flour 2.5kg"] = (35, 50, "bag"),
            
            // Fresh Produce
            ["Potatoes 5kg"] = (40, 60, "bag"),
            ["Onions 2kg"] = (20, 35, "bag"),
            ["Tomatoes 1kg"] = (15, 30, "kg"),
            ["Cabbage"] = (15, 25, "each"),
            ["Bananas"] = (20, 35, "dozen"),
            ["Apples"] = (30, 50, "kg"),
            
            // Beverages
            ["Coca-Cola 2L"] = (18, 25, "bottle"),
            ["Milk 1L"] = (15, 22, "carton"),
            ["Black Tea 100g"] = (15, 25, "box"),
            ["Coffee Instant 200g"] = (45, 70, "jar"),
            
            // Snacks
            ["Simba Chips"] = (8, 15, "packet"),
            ["Chocolate Bar"] = (10, 18, "bar"),
            ["Biscuits"] = (12, 20, "packet"),
            
            // Household
            ["Sunlight Soap"] = (10, 15, "bar"),
            ["Washing Powder 2kg"] = (45, 70, "box"),
            ["Dishwashing Liquid"] = (20, 30, "bottle"),
            
            // Personal Care
            ["Toothpaste"] = (15, 28, "tube"),
            ["Soap Bar"] = (8, 15, "bar"),
            ["Shampoo 400ml"] = (35, 55, "bottle"),
            
            // Airtime
            ["Vodacom Airtime"] = (5, 500, "voucher"),
            ["MTN Airtime"] = (5, 500, "voucher")
        };

        var productFaker = new Faker<Product>()
            .RuleFor(p => p.CategoryId, f => f.PickRandom(categories).Id)
            .RuleFor(p => p.IsActive, true)
            .RuleFor(p => p.Currency, "ZAR")
            .RuleFor(p => p.IsTaxable, f => f.Random.Bool(0.9f))
            .RuleFor(p => p.MinimumStockLevel, f => f.Random.Int(5, 20))
            .RuleFor(p => p.ReorderQuantity, f => f.Random.Int(20, 100));

        var products = new List<Product>();
        
        foreach (var (name, priceRange) in productNames)
        {
            var product = productFaker.Generate();
            product.Name = name;
            product.SKU = $"SKU-{Guid.NewGuid().ToString()[..8].ToUpper()}";
            product.Barcode = new Faker().Commerce.Ean13();
            product.BasePrice = new Faker().Random.Decimal(priceRange.min, priceRange.max);
            product.CostPrice = product.BasePrice * 0.7m; // 30% markup
            product.Unit = priceRange.unit;
            product.Description = new Faker().Commerce.ProductDescription();
            products.Add(product);
        }

        _context.Set<Product>().AddRange(products);
        await _context.SaveChangesAsync();

        // Create stock levels for each product in each store
        // Ensure all products have stock > 0
        var stockLevels = new List<StockLevel>();
        foreach (var product in products)
        {
            foreach (var store in stores)
            {
                stockLevels.Add(new StockLevel
                {
                    ProductId = product.Id,
                    ShopId = store.Id,
                    CurrentStock = new Faker().Random.Int(1, 100), // Always > 0
                    ReservedStock = 0,
                    ReorderPoint = product.MinimumStockLevel,
                    ReorderQuantity = product.ReorderQuantity ?? 50
                });
            }
        }

        _context.Set<StockLevel>().AddRange(stockLevels);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("✅ Seeded {ProductCount} products with {StockCount} stock level records.", 
            products.Count, stockLevels.Count);
    }

    private async Task SeedVendorsAsync()
    {
        var existingCount = await _context.Set<Vendor>().CountAsync();
        if (existingCount >= 15)
        {
            _logger.LogInformation("✅ Vendors already seeded ({Count} existing).", existingCount);
            return;
        }

        var vendorFaker = new Faker<Vendor>()
            .RuleFor(v => v.Name, f => f.Company.CompanyName())
            .RuleFor(v => v.Description, f => f.Company.CatchPhrase())
            .RuleFor(v => v.Email, f => f.Internet.Email())
            .RuleFor(v => v.ContactPerson, f => f.Name.FullName())
            .RuleFor(v => v.ContactPhone, f => new PhoneNumber($"+27{f.Random.Int(6, 8)}{f.Random.Int(10000000, 99999999)}"))
            .RuleFor(v => v.Website, f => f.Internet.Url())
            .RuleFor(v => v.CompanyRegNumber, f => f.Random.Replace("20##/######/##"))
            .RuleFor(v => v.VATNumber, f => f.Random.Replace("4##########"))
            .RuleFor(v => v.Active, true)
            .RuleFor(v => v.CreditLimit, f => f.Random.Decimal(50000, 500000))
            .RuleFor(v => v.PaymentTermsDays, f => f.PickRandom(new[] { 7, 14, 30, 60 }))
            .RuleFor(v => v.DisplayOrder, f => f.IndexFaker);

        var vendors = vendorFaker.Generate(15);

        // Create addresses for vendors
        foreach (var vendor in vendors)
        {
            vendor.Address = new Address
            {
                Street = new Faker().Address.StreetAddress(),
                City = new Faker().Address.City(),
                StateProvince = new Faker().PickRandom(new[] { "Gauteng", "Western Cape", "KwaZulu-Natal" }),
                PostalCode = new Faker().Address.ZipCode(),
                Country = "ZA"
            };
        }

        _context.Set<Vendor>().AddRange(vendors);
        await _context.SaveChangesAsync();
        _logger.LogInformation("✅ Seeded {Count} vendors.", vendors.Count);
    }

    private async Task SeedCustomersAsync()
    {
        var existingCount = await _context.Set<Customer>().CountAsync();
        if (existingCount >= 100)
        {
            _logger.LogInformation("✅ Customers already seeded ({Count} existing).", existingCount);
            return;
        }

        var stores = await _context.Stores.ToListAsync();
        if (!stores.Any())
        {
            _logger.LogWarning("⚠️  No stores found. Skipping customer seeding.");
            return;
        }

        var customerFaker = new Faker<Customer>()
            .RuleFor(c => c.ShopId, f => f.PickRandom(stores).Id)
            .RuleFor(c => c.FirstName, f => f.Name.FirstName())
            .RuleFor(c => c.LastName, f => f.Name.LastName())
            .RuleFor(c => c.Phone, f => new PhoneNumber($"+27{f.Random.Int(6, 8)}{f.Random.Int(10000000, 99999999)}"))
            .RuleFor(c => c.Email, (f, c) => f.Internet.Email(c.FirstName, c.LastName))
            .RuleFor(c => c.IsActive, true)
            .RuleFor(c => c.AllowsMarketing, f => f.Random.Bool(0.6f))
            .RuleFor(c => c.TotalPurchaseAmount, f => f.Random.Decimal(0, 5000))
            .RuleFor(c => c.TotalPurchaseCount, f => f.Random.Int(0, 50))
            .RuleFor(c => c.FirstPurchaseDate, f => DateTimeOffset.UtcNow.AddDays(-f.Random.Int(30, 365)))
            .RuleFor(c => c.LastPurchaseDate, f => DateTimeOffset.UtcNow.AddDays(-f.Random.Int(0, 30)));

        var customers = customerFaker.Generate(100);

        // Create addresses for some customers
        foreach (var customer in customers.Where(c => new Faker().Random.Bool(0.7f)))
        {
            customer.Address = new Address
            {
                Street = new Faker().Address.StreetAddress(),
                City = new Faker().Address.City(),
                StateProvince = "Gauteng",
                PostalCode = new Faker().Address.ZipCode(),
                Country = "ZA"
            };
        }

        _context.Set<Customer>().AddRange(customers);
        await _context.SaveChangesAsync();
        _logger.LogInformation("✅ Seeded {Count} customers.", customers.Count);
    }

    private async Task SeedDriversAsync()
    {
        var existingCount = await _context.Set<Driver>().CountAsync();
        if (existingCount >= 8)
        {
            _logger.LogInformation("✅ Drivers already seeded ({Count} existing).", existingCount);
            return;
        }

        var driverFaker = new Faker<Driver>()
            .RuleFor(d => d.FirstName, f => f.Name.FirstName())
            .RuleFor(d => d.LastName, f => f.Name.LastName())
            .RuleFor(d => d.Phone, f => new PhoneNumber($"+27{f.Random.Int(6, 8)}{f.Random.Int(10000000, 99999999)}"))
            .RuleFor(d => d.Email, (f, d) => f.Internet.Email(d.FirstName, d.LastName))
            .RuleFor(d => d.LicenseNumber, f => f.Random.Replace("########"))
            .RuleFor(d => d.VehicleType, f => f.PickRandom("Sedan", "Bakkie", "Van", "Motorcycle"))
            .RuleFor(d => d.VehicleRegistration, f => f.Random.Replace("??-##-??-??").ToUpper())
            .RuleFor(d => d.IsActive, true)
            .RuleFor(d => d.IsAvailable, f => f.Random.Bool(0.8f));

        var drivers = driverFaker.Generate(8);

        _context.Set<Driver>().AddRange(drivers);
        await _context.SaveChangesAsync();
        _logger.LogInformation("✅ Seeded {Count} drivers.", drivers.Count);
    }

    private async Task SeedPurchaseOrdersAsync()
    {
        var existingCount = await _context.Set<PurchaseOrder>().CountAsync();
        if (existingCount >= 30)
        {
            _logger.LogInformation("✅ Purchase orders already seeded ({Count} existing).", existingCount);
            return;
        }

        var stores = await _context.Stores.ToListAsync();
        var vendors = await _context.Set<Vendor>().ToListAsync();
        var products = await _context.Set<Product>().ToListAsync();

        if (!stores.Any() || !vendors.Any() || !products.Any())
        {
            _logger.LogWarning("⚠️  Missing required data (stores/vendors/products). Skipping purchase order seeding.");
            return;
        }

        var poFaker = new Faker<PurchaseOrder>()
            .RuleFor(po => po.PONumber, f => $"PO-{f.Random.Number(1000, 9999)}")
            .RuleFor(po => po.ShopId, f => f.PickRandom(stores).Id)
            .RuleFor(po => po.VendorId, f => f.PickRandom(vendors).Id)
            .RuleFor(po => po.OrderDate, f => DateTimeOffset.UtcNow.AddDays(-f.Random.Int(1, 60)))
            .RuleFor(po => po.ExpectedDeliveryDate, (f, po) => po.OrderDate.AddDays(f.Random.Int(3, 14)))
            .RuleFor(po => po.Status, f => f.PickRandom<PurchaseOrderStatus>())
            .RuleFor(po => po.IsPartOfGroupBuy, f => f.Random.Bool(0.3f));

        var purchaseOrders = poFaker.Generate(30);

        foreach (var po in purchaseOrders)
        {
            // Add 2-5 items to each PO
            var itemCount = new Faker().Random.Int(2, 5);
            var poProducts = new Faker().PickRandom(products, itemCount).ToList();

            foreach (var product in poProducts)
            {
                var quantity = new Faker().Random.Int(10, 100);
                var unitPrice = product.CostPrice ?? product.BasePrice * 0.7m;
                var lineSubtotal = quantity * unitPrice;
                var taxAmount = product.IsTaxable ? lineSubtotal * 0.15m : 0;
                
                var item = new PurchaseOrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductSKU = product.SKU,
                    QuantityOrdered = quantity,
                    QuantityReceived = 0,
                    UnitPrice = unitPrice,
                    TaxAmount = taxAmount,
                    LineTotal = lineSubtotal + taxAmount
                };
                
                po.Items.Add(item);
            }

            // Calculate totals
            po.Subtotal = po.Items.Sum(i => i.QuantityOrdered * i.UnitPrice);
            po.TaxAmount = po.Items.Sum(i => i.TaxAmount);
            po.ShippingCost = new Faker().Random.Decimal(0, 500);
            po.Total = po.Subtotal + po.TaxAmount + po.ShippingCost;

            if (po.Status == PurchaseOrderStatus.Approved || po.Status == PurchaseOrderStatus.PartiallyReceived)
            {
                po.ApprovedDate = DateTime.UtcNow.AddDays(-new Faker().Random.Int(0, 30));
                po.ApprovedBy = "administrator@localhost";
            }
        }

        _context.Set<PurchaseOrder>().AddRange(purchaseOrders);
        await _context.SaveChangesAsync();
        _logger.LogInformation("✅ Seeded {Count} purchase orders.", purchaseOrders.Count);
    }

    private async Task SeedSalesAsync()
    {
        var existingCount = await _context.Set<Sale>().CountAsync();
        if (existingCount >= 200)
        {
            _logger.LogInformation("✅ Sales already seeded ({Count} existing).", existingCount);
            return;
        }

        var stores = await _context.Stores.ToListAsync();
        var customers = await _context.Set<Customer>().ToListAsync();
        var products = await _context.Set<Product>().ToListAsync();

        if (!stores.Any() || !products.Any())
        {
            _logger.LogWarning("⚠️  Missing required data (stores/products). Skipping sales seeding.");
            return;
        }

        // Generate unique sale numbers using counter to avoid duplicates
        var saleNumberCounter = 10000;
        var saleFaker = new Faker<Sale>()
            .RuleFor(s => s.SaleNumber, f => $"SALE-{saleNumberCounter++}")
            .RuleFor(s => s.ShopId, f => f.PickRandom(stores).Id)
            .RuleFor(s => s.CustomerId, f => f.Random.Bool(0.7f) && customers.Any() ? f.PickRandom(customers).Id : null)
            .RuleFor(s => s.SaleDate, f => DateTimeOffset.UtcNow.AddDays(-f.Random.Int(0, 90)))
            .RuleFor(s => s.Status, f => f.PickRandom<SaleStatus>())
            .RuleFor(s => s.PaymentMethod, f => f.PickRandom<PaymentType>());

        var sales = saleFaker.Generate(200);

        foreach (var sale in sales)
        {
            // Add 1-8 items to each sale
            var itemCount = new Faker().Random.Int(1, 8);
            var saleProducts = new Faker().PickRandom(products, itemCount).ToList();

            foreach (var product in saleProducts)
            {
                var quantity = new Faker().Random.Int(1, 5);
                var unitPrice = product.BasePrice;
                var lineSubtotal = quantity * unitPrice;
                var taxAmount = product.IsTaxable ? lineSubtotal * 0.15m : 0;
                
                var item = new SaleItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductSKU = product.SKU,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    DiscountAmount = 0,
                    TaxAmount = taxAmount,
                    LineTotal = lineSubtotal + taxAmount
                };
                
                ((List<SaleItem>)sale.Items).Add(item);
            }

            // Calculate totals
            sale.Subtotal = sale.Items.Sum(i => i.Quantity * i.UnitPrice);
            sale.TaxAmount = sale.Items.Sum(i => i.TaxAmount);
            sale.DiscountAmount = new Faker().Random.Bool(0.2f) ? sale.Subtotal * 0.05m : 0; // 5% discount sometimes
            sale.Total = sale.Subtotal + sale.TaxAmount - sale.DiscountAmount;

            if (sale.PaymentMethod != PaymentType.Cash)
            {
                sale.PaymentReference = $"REF-{new Faker().Random.AlphaNumeric(10).ToUpper()}";
            }
        }

        _context.Set<Sale>().AddRange(sales);
        await _context.SaveChangesAsync();
        _logger.LogInformation("✅ Seeded {Count} sales.", sales.Count);
    }

    private async Task SeedOrdersAsync()
    {
        var existingCount = await _context.Set<Order>().CountAsync();
        if (existingCount >= 100)
        {
            _logger.LogInformation("✅ Orders already seeded ({Count} existing).", existingCount);
            return;
        }

        var customers = await _context.Set<Customer>().ToListAsync();
        var products = await _context.Set<Product>().ToListAsync();

        if (!customers.Any() || !products.Any())
        {
            _logger.LogWarning("⚠️  Missing required data (customers/products). Skipping orders seeding.");
            return;
        }

        var orderFaker = new Faker<Order>()
            .RuleFor(o => o.CustomerId, f => f.PickRandom(customers).Id)
            .RuleFor(o => o.OrderStatus, f => f.PickRandom<OrderStatus>())
            .RuleFor(o => o.PaymentStatus, f => f.PickRandom<Domain.Enums.PaymentStatus>())
            .RuleFor(o => o.ShippingStatus, f => f.PickRandom<ShippingStatus>())
            .RuleFor(o => o.PaymentMethodSystemName, f => f.PickRandom("Cash", "Card", "BankTransfer", "Wallet"))
            .RuleFor(o => o.ShippingMethod, f => f.PickRandom("Standard", "Express", "SameDay", null))
            .RuleFor(o => o.CustomerCurrencyCode, "ZAR")
            .RuleFor(o => o.CustomerTaxDisplayType, TaxDisplayType.IncludingTax)
            .RuleFor(o => o.CustomerIp, f => f.Internet.Ip())
            .RuleFor(o => o.Deleted, false);

        var orders = new List<Order>();
        var ordersToCreate = Math.Max(0, 100 - existingCount);

        for (int i = 0; i < ordersToCreate; i++)
        {
            var order = orderFaker.Generate();
            
            // Set Created date (from BaseAuditableEntity) to random date in past 90 days
            // DateTimeOffset.UtcNow is already UTC, but ensure explicit UTC conversion for PostgreSQL
            var createdDate = DateTimeOffset.UtcNow.AddDays(-new Faker().Random.Int(0, 90));
            order.Created = createdDate.ToUniversalTime(); // Explicit UTC conversion
            order.CreatedBy = null;
            order.LastModified = createdDate.ToUniversalTime(); // Explicit UTC conversion
            order.LastModifiedBy = null;

            // Add 1-8 items to each order
            var itemCount = new Faker().Random.Int(1, 8);
            var orderProducts = new Faker().PickRandom(products, itemCount).ToList();

            decimal subtotalExclTax = 0;
            decimal subtotalInclTax = 0;
            decimal taxAmount = 0;

            foreach (var product in orderProducts)
            {
                var quantity = new Faker().Random.Int(1, 5);
                var unitPriceExclTax = product.BasePrice;
                var taxRate = product.IsTaxable ? 0.15m : 0m;
                var unitTax = unitPriceExclTax * taxRate;
                var unitPriceInclTax = unitPriceExclTax + unitTax;

                var linePriceExclTax = quantity * unitPriceExclTax;
                var lineTax = linePriceExclTax * taxRate;
                var linePriceInclTax = linePriceExclTax + lineTax;

                subtotalExclTax += linePriceExclTax;
                taxAmount += lineTax;
                subtotalInclTax += linePriceInclTax;

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = quantity,
                    UnitPriceExclTax = unitPriceExclTax,
                    UnitPriceInclTax = unitPriceInclTax,
                    PriceExclTax = linePriceExclTax,
                    PriceInclTax = linePriceInclTax,
                    DiscountAmountExclTax = 0,
                    DiscountAmountInclTax = 0
                };

                order.OrderItems.Add(orderItem);
            }

            // Calculate order totals
            order.OrderSubtotalExclTax = subtotalExclTax;
            order.OrderSubtotalInclTax = subtotalInclTax;
            order.OrderTax = taxAmount;
            order.OrderTotal = subtotalInclTax;

            // Set PaidDate if payment is completed
            if (order.PaymentStatus == Domain.Enums.PaymentStatus.Completed || 
                order.PaymentStatus == Domain.Enums.PaymentStatus.Captured)
            {
                // Use UtcDateTime to ensure UTC kind (PostgreSQL requirement)
                order.PaidDateUtc = createdDate.AddMinutes(new Faker().Random.Int(1, 60)).UtcDateTime;
            }

            // Set shipping status based on order status
            if (order.OrderStatus == OrderStatus.Complete)
            {
                order.ShippingStatus = ShippingStatus.Delivered;
            }
            else if (order.OrderStatus == OrderStatus.Processing)
            {
                order.ShippingStatus = ShippingStatus.Shipped;
            }

            orders.Add(order);
        }

        _context.Set<Order>().AddRange(orders);
        await _context.SaveChangesAsync();
        _logger.LogInformation("✅ Seeded {Count} orders.", orders.Count);
    }

    private async Task SeedPaymentsAsync()
    {
        var existingCount = await _context.Set<Payment>().CountAsync();
        if (existingCount >= 100)
        {
            _logger.LogInformation("✅ Payments already seeded ({Count} existing).", existingCount);
            return;
        }

        var sales = await _context.Set<Sale>()
            .Where(s => s.Status == SaleStatus.Completed)
            .ToListAsync();

        if (!sales.Any())
        {
            _logger.LogWarning("⚠️  No completed sales found. Skipping payment seeding.");
            return;
        }

        var paymentFaker = new Faker<Payment>()
            .RuleFor(p => p.PaymentType, f => f.PickRandom<PaymentType>())
            .RuleFor(p => p.Status, PaymentStatus.Completed)
            .RuleFor(p => p.PaymentDate, (f, p) => DateTimeOffset.UtcNow.AddDays(-f.Random.Int(0, 90)))
            .RuleFor(p => p.TransactionRef, f => $"TXN-{f.Random.AlphaNumeric(12).ToUpper()}")
            .RuleFor(p => p.PaymentReference, f => $"PAY-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(p => p.InitiatedAt, f => DateTimeOffset.UtcNow.AddDays(-f.Random.Int(0, 90)))
            .RuleFor(p => p.AuthorizedAt, (f, p) => p.InitiatedAt.AddSeconds(f.Random.Int(1, 60)))
            .RuleFor(p => p.CapturedAt, (f, p) => p.AuthorizedAt?.AddSeconds(f.Random.Int(1, 60)));

        var payments = new List<Payment>();
        
        foreach (var sale in sales.Take(100)) // Create payments for first 100 completed sales
        {
            var payment = paymentFaker.Generate();
            payment.Amount = sale.Total;
            payment.Currency = "ZAR";
            payment.ShopId = sale.ShopId;
            payment.SaleId = sale.Id;
            payment.CustomerId = sale.CustomerId;
            payment.SourceType = "Sale";
            payment.SourceId = sale.Id;
            payment.Notes = $"Payment for {sale.SaleNumber}";
            payments.Add(payment);
        }

        _context.Set<Payment>().AddRange(payments);
        await _context.SaveChangesAsync();
        _logger.LogInformation("✅ Seeded {Count} payments.", payments.Count);
    }

    private async Task<bool> CheckIdentityTablesExistAsync()
    {
        try
        {
            // Check if AspNetRoles table exists by querying PostgreSQL system catalog
            // Using raw SQL to avoid any EF Core navigation that might fail
            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();
            
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM information_schema.tables WHERE table_name = 'AspNetRoles' AND table_schema = 'public'";
            
            var result = await command.ExecuteScalarAsync();
            var tableCount = Convert.ToInt32(result);
            
            if (tableCount == 0)
            {
                _logger.LogWarning("Identity tables (AspNetRoles) do not exist in the database.");
            }
            
            return tableCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not check for Identity tables existence.");
            return false;
        }
    }
}
