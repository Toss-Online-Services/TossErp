using TossErp.CRM.Domain.Aggregates;
using TossErp.CRM.Domain.Enums;
using TossErp.CRM.Domain.ValueObjects;
using Crm.Domain.Entities;
using Crm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Customer = TossErp.CRM.Domain.Aggregates.Customer;

namespace Crm.Infrastructure.Data.Seeders;

public class CrmDataSeeder
{
    private readonly CrmDbContext _context;
    private readonly ILogger<CrmDataSeeder> _logger;

    public CrmDataSeeder(CrmDbContext context, ILogger<CrmDataSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("Starting CRM data seeding...");

            // Check if data already exists
            if (await _context.Customers.AnyAsync())
            {
                _logger.LogInformation("CRM data already exists, skipping seeding");
                return;
            }

            var tenantId = Guid.NewGuid(); // Demo tenant
            var seederId = "DataSeeder";

            // Seed Customers
            var customers = await SeedCustomersAsync(tenantId, seederId);
            
            // Seed Leads
            var leads = await SeedLeadsAsync(tenantId, seederId);
            
            // Seed Opportunities
            await SeedOpportunitiesAsync(tenantId, seederId, customers);

            await _context.SaveChangesAsync();
            _logger.LogInformation("CRM data seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during CRM data seeding");
            throw;
        }
    }

    private async Task<List<Customer>> SeedCustomersAsync(Guid tenantId, string createdBy)
    {
        _logger.LogInformation("Seeding customers...");

        var customers = new List<Customer>
        {
            // Marvel-themed customers for a tech consulting company
            new Customer(
                Guid.NewGuid(),
                tenantId,
                new CustomerNumber("CUST-001"),
                "Stark Industries",
                CustomerType.Enterprise,
                createdBy,
                LeadSource.Website,
                new EmailAddress("contact@starkindustries.com"),
                new PhoneNumber("+1-555-STARK"),
                "Technology"
            ),

            new Customer(
                Guid.NewGuid(),
                tenantId,
                new CustomerNumber("CUST-002"),
                "Parker Technologies",
                CustomerType.Enterprise,
                createdBy,
                LeadSource.Referral,
                new EmailAddress("info@parkertech.com"),
                new PhoneNumber("+1-555-PARKER"),
                "Software Development"
            ),

            new Customer(
                Guid.NewGuid(),
                tenantId,
                new CustomerNumber("CUST-003"),
                "Banner Labs",
                CustomerType.Enterprise,
                createdBy,
                LeadSource.TradeShow,
                new EmailAddress("research@bannerlabs.com"),
                new PhoneNumber("+1-555-BANNER"),
                "Research & Development"
            ),

            new Customer(
                Guid.NewGuid(),
                tenantId,
                new CustomerNumber("CUST-004"),
                "Rogers Communications",
                CustomerType.Enterprise,
                createdBy,
                LeadSource.Advertisement,
                new EmailAddress("steve@rogerscomm.com"),
                new PhoneNumber("+1-555-ROGERS"),
                "Communications"
            ),

            new Customer(
                Guid.NewGuid(),
                tenantId,
                new CustomerNumber("CUST-005"),
                "Romanoff Security",
                CustomerType.SmallBusiness,
                createdBy,
                LeadSource.ColdCall,
                new EmailAddress("natasha@romanoffsec.com"),
                new PhoneNumber("+1-555-NATASHA"),
                "Cybersecurity"
            )
        };

        _context.Customers.AddRange(customers);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Seeded {customers.Count} customers");
        return customers;
    }

    private async Task<List<Lead>> SeedLeadsAsync(Guid tenantId, string createdBy)
    {
        _logger.LogInformation("Seeding leads...");

        var leads = new List<Lead>
        {
            new Lead(
                Guid.NewGuid(),
                tenantId,
                "Thor",
                "Odinson",
                "Asgard Enterprises",
                new EmailAddress("thor@asgard.com"),
                LeadSource.Website,
                createdBy,
                "Chief Executive Officer",
                new PhoneNumber("+1-555-THUNDER"),
                "Entertainment",
                500,
                null,
                "Digital Transformation Campaign"
            ),

            new Lead(
                Guid.NewGuid(),
                tenantId,
                "Carol",
                "Danvers",
                "Marvel Airlines",
                new EmailAddress("carol@marvelairlines.com"),
                LeadSource.SocialMedia,
                createdBy,
                "Captain",
                new PhoneNumber("+1-555-MARVEL"),
                "Aviation",
                250,
                null,
                "Cloud Migration Campaign"
            ),

            new Lead(
                Guid.NewGuid(),
                tenantId,
                "Stephen",
                "Strange",
                "Sanctum Consulting",
                new EmailAddress("stephen@sanctum.com"),
                LeadSource.Referral,
                createdBy,
                "Master Consultant",
                new PhoneNumber("+1-555-SANCTUM"),
                "Professional Services",
                100,
                null,
                "AI Implementation Campaign"
            ),

            new Lead(
                Guid.NewGuid(),
                tenantId,
                "Scott",
                "Lang",
                "Ant-Man Solutions",
                new EmailAddress("scott@antmansolutions.com"),
                LeadSource.Email,
                createdBy,
                "CEO",
                new PhoneNumber("+1-555-ANTMAN"),
                "Technology",
                50,
                null,
                "Microservices Campaign"
            ),

            new Lead(
                Guid.NewGuid(),
                tenantId,
                "Wanda",
                "Maximoff",
                "Hex Technologies",
                new EmailAddress("wanda@hextech.com"),
                LeadSource.Advertisement,
                createdBy,
                "CTO",
                new PhoneNumber("+1-555-HEXTECH"),
                "Software",
                75,
                null,
                "DevOps Transformation Campaign"
            )
        };

        _context.Leads.AddRange(leads);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Seeded {leads.Count} leads");
        return leads;
    }

    private async Task SeedOpportunitiesAsync(Guid tenantId, string createdBy, List<Customer> customers)
    {
        _logger.LogInformation("Seeding opportunities...");

        var opportunities = new List<Opportunity>
        {
            new Opportunity(
                Guid.NewGuid(),
                tenantId,
                "Stark Industries - ERP Implementation",
                customers[0].Id,
                new OpportunityValue(
                    new Money(250000, "USD"),
                    80
                ),
                DateTime.UtcNow.AddDays(45),
                createdBy,
                "Complete ERP system implementation with custom modules for technology company",
                OpportunityType.NewBusiness,
                OpportunityPriority.High,
                LeadSource.Website,
                null,
                null,
                "Enterprise Solutions Campaign"
            ),

            new Opportunity(
                Guid.NewGuid(),
                tenantId,
                "Parker Technologies - Cloud Migration",
                customers[1].Id,
                new OpportunityValue(
                    new Money(150000, "USD"),
                    60
                ),
                DateTime.UtcNow.AddDays(30),
                createdBy,
                "Migration of legacy systems to cloud infrastructure with DevOps setup",
                OpportunityType.Upsell,
                OpportunityPriority.High,
                LeadSource.Referral,
                null,
                null,
                "Cloud First Campaign"
            ),

            new Opportunity(
                Guid.NewGuid(),
                tenantId,
                "Banner Labs - Research Platform",
                customers[2].Id,
                new OpportunityValue(
                    new Money(300000, "USD"),
                    45
                ),
                DateTime.UtcNow.AddDays(60),
                createdBy,
                "Custom research data platform with AI/ML capabilities for scientific research",
                OpportunityType.NewBusiness,
                OpportunityPriority.Medium,
                LeadSource.TradeShow,
                null,
                null,
                "Innovation Labs Campaign"
            ),

            new Opportunity(
                Guid.NewGuid(),
                tenantId,
                "Rogers Communications - Security Upgrade",
                customers[3].Id,
                new OpportunityValue(
                    new Money(75000, "USD"),
                    90
                ),
                DateTime.UtcNow.AddDays(15),
                createdBy,
                "Cybersecurity infrastructure upgrade and penetration testing",
                OpportunityType.Renewal,
                OpportunityPriority.High,
                LeadSource.Advertisement
            ),

            new Opportunity(
                Guid.NewGuid(),
                tenantId,
                "Romanoff Security - Incident Response System",
                customers[4].Id,
                new OpportunityValue(
                    new Money(125000, "USD"),
                    70
                ),
                DateTime.UtcNow.AddDays(90),
                createdBy,
                "24/7 incident response and monitoring system implementation",
                OpportunityType.CrossSell,
                OpportunityPriority.Medium,
                LeadSource.ColdCall,
                null,
                null,
                "Security Excellence Campaign"
            ),

            new Opportunity(
                Guid.NewGuid(),
                tenantId,
                "Multi-Client Analytics Platform",
                customers[0].Id,
                new OpportunityValue(
                    new Money(500000, "USD"),
                    25
                ),
                DateTime.UtcNow.AddDays(120),
                createdBy,
                "Enterprise-wide analytics platform serving multiple business units",
                OpportunityType.NewBusiness,
                OpportunityPriority.Low,
                LeadSource.Website,
                null,
                null,
                "Data Revolution Campaign"
            )
        };

        _context.Opportunities.AddRange(opportunities);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Seeded {opportunities.Count} opportunities");
    }
}
