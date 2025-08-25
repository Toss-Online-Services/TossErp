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

            // Ensure database is created
            await _context.Database.EnsureCreatedAsync();

            // Check if data already exists
            if (await _context.Customers.AnyAsync() || 
                await _context.Leads.AnyAsync() || 
                await _context.Opportunities.AnyAsync())
            {
                _logger.LogInformation("Data already exists, skipping seeding");
                return;
            }

            // Seed customers first
            var customers = await SeedCustomersAsync();
            
            // Seed leads
            var leads = await SeedLeadsAsync();

            // Seed opportunities
            await SeedOpportunitiesAsync(customers, leads);

            await _context.SaveChangesAsync();
            _logger.LogInformation("CRM data seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during CRM data seeding");
            throw;
        }
    }

    private async Task<List<Customer>> SeedCustomersAsync()
    {
        _logger.LogInformation("Seeding customers...");

        var customers = new List<Customer>
        {
            Customer.CreateNewCustomer(
                firstName: "John",
                lastName: "Doe", 
                email: "john.doe@example.com",
                phone: "+1234567890",
                address: "123 Main St, New York, NY 10001, USA",
                dateOfBirth: new DateTime(1985, 5, 15)
            ),
            Customer.CreateNewCustomer(
                firstName: "Jane",
                lastName: "Smith",
                email: "jane.smith@techcorp.com",
                phone: "+1987654321",
                address: "456 Tech Ave, San Francisco, CA 94105, USA",
                dateOfBirth: new DateTime(1990, 8, 22)
            ),
            Customer.CreateNewCustomer(
                firstName: "Michael",
                lastName: "Johnson",
                email: "m.johnson@consulting.com",
                phone: "+1122334455",
                address: "789 Business Blvd, Chicago, IL 60601, USA",
                dateOfBirth: new DateTime(1982, 11, 30)
            ),
            Customer.CreateNewCustomer(
                firstName: "Emily",
                lastName: "Davis",
                email: "emily.davis@startup.io",
                phone: "+1555666777",
                address: "321 Innovation Dr, Austin, TX 78701, USA",
                dateOfBirth: new DateTime(1988, 3, 12)
            ),
            Customer.CreateNewCustomer(
                firstName: "Robert",
                lastName: "Wilson",
                email: "r.wilson@manufacturing.com",
                phone: "+1666777888",
                address: "654 Industrial Way, Detroit, MI 48201, USA",
                dateOfBirth: new DateTime(1975, 9, 18)
            )
        };

        // Update some customers with purchase history
        customers[0].RecordPurchase(2500.00m, DateTime.UtcNow.AddDays(-30));
        customers[0].RecordPurchase(1800.50m, DateTime.UtcNow.AddDays(-15));
        customers[1].RecordPurchase(5000.00m, DateTime.UtcNow.AddDays(-45));
        customers[2].RecordPurchase(3200.75m, DateTime.UtcNow.AddDays(-20));

        await _context.Customers.AddRangeAsync(customers);
        return customers;
    }

    private async Task<List<Lead>> SeedLeadsAsync()
    {
        _logger.LogInformation("Seeding leads...");

        var tenantId = Guid.NewGuid(); // In real scenario, this would come from context
        var leads = new List<Lead>();

        // High-quality leads
        var lead1 = Lead.CreateNewLead(
            tenantId: tenantId,
            firstName: "Sarah",
            lastName: "Connor",
            company: "Cyberdyne Systems",
            email: new EmailAddress("sarah.connor@cyberdyne.com"),
            source: LeadSource.Website,
            createdBy: "System",
            jobTitle: "CTO",
            phone: new PhoneNumber("+1800FUTURE"),
            industry: "Technology",
            companySize: 500
        );
        lead1.UpdateScore(85, "High engagement score", "System");
        lead1.QualifyLead("Meets all criteria", "System");
        leads.Add(lead1);

        var lead2 = Lead.CreateNewLead(
            tenantId: tenantId,
            firstName: "Tony",
            lastName: "Stark",
            company: "Stark Industries",
            email: new EmailAddress("tony@starkindustries.com"),
            source: LeadSource.SocialMedia,
            createdBy: "System",
            jobTitle: "CEO",
            phone: new PhoneNumber("+1IRONMAN1"),
            industry: "Manufacturing",
            companySize: 2000
        );
        lead2.UpdateScore(92, "Very high engagement", "System");
        leads.Add(lead2);

        // Medium-quality leads
        var lead3 = Lead.CreateNewLead(
            tenantId: tenantId,
            firstName: "Bruce",
            lastName: "Wayne",
            company: "Wayne Enterprises",
            email: new EmailAddress("bruce.wayne@wayneent.com"),
            source: LeadSource.Referral,
            createdBy: "System",
            jobTitle: "Chairman",
            phone: new PhoneNumber("+1BATMAN01"),
            industry: "Conglomerate",
            companySize: 5000
        );
        lead3.UpdateScore(68, "Good potential", "System");
        leads.Add(lead3);

        var lead4 = Lead.CreateNewLead(
            tenantId: tenantId,
            firstName: "Peter",
            lastName: "Parker",
            company: "Daily Bugle",
            email: new EmailAddress("p.parker@dailybugle.com"),
            source: LeadSource.ColdCall,
            createdBy: "System",
            jobTitle: "Photographer",
            phone: new PhoneNumber("+1SPIDEY99"),
            industry: "Media",
            companySize: 50
        );
        lead4.UpdateScore(45, "Moderate interest", "System");
        leads.Add(lead4);

        // New/unqualified leads
        var lead5 = Lead.CreateNewLead(
            tenantId: tenantId,
            firstName: "Clark",
            lastName: "Kent",
            company: "Daily Planet",
            email: new EmailAddress("clark.kent@dailyplanet.com"),
            source: LeadSource.TradeShow,
            createdBy: "System",
            jobTitle: "Reporter",
            phone: new PhoneNumber("+1SUPERMAN"),
            industry: "Media",
            companySize: 200
        );
        leads.Add(lead5);

        var lead6 = Lead.CreateNewLead(
            tenantId: tenantId,
            firstName: "Diana",
            lastName: "Prince",
            company: "Themyscira Consulting",
            email: new EmailAddress("diana@themyscira.com"),
            source: LeadSource.LinkedIn,
            createdBy: "System",
            jobTitle: "CEO",
            phone: new PhoneNumber("+1WONDERW1"),
            industry: "Consulting",
            companySize: 25
        );
        lead6.UpdateScore(75, "Strong initial engagement", "System");
        leads.Add(lead6);

        await _context.Leads.AddRangeAsync(leads);
        return leads;
    }

    private async Task SeedOpportunitiesAsync(List<Customer> customers, List<Lead> leads)
    {
        _logger.LogInformation("Seeding opportunities...");

        var tenantId = leads.First().TenantId;
        var opportunities = new List<Opportunity>();

        // High-value opportunities from converted leads
        var opp1 = Opportunity.CreateNewOpportunity(
            tenantId: tenantId,
            name: "Cyberdyne ERP Implementation",
            customerId: customers[1].Id, // Jane Smith
            value: new OpportunityValue(
                new Money(250000m, "USD"), 
                85m
            ),
            expectedCloseDate: DateTime.UtcNow.AddDays(45),
            createdBy: "System",
            leadId: leads[0].Id,
            description: "Complete ERP system implementation for Cyberdyne Systems",
            type: OpportunityType.NewBusiness,
            priority: OpportunityPriority.High
        );
        opp1.AdvanceToStage(OpportunityStage.Proposal, "Proposal submitted", "System");
        opportunities.Add(opp1);

        var opp2 = Opportunity.CreateNewOpportunity(
            tenantId: tenantId,
            name: "Stark Industries Digital Transformation",
            customerId: customers[2].Id, // Michael Johnson
            value: new OpportunityValue(
                new Money(500000m, "USD"), 
                70m
            ),
            expectedCloseDate: DateTime.UtcNow.AddDays(60),
            createdBy: "System",
            leadId: leads[1].Id,
            description: "Digital transformation initiative for Stark Industries",
            type: OpportunityType.NewBusiness,
            priority: OpportunityPriority.High
        );
        opp2.AdvanceToStage(OpportunityStage.Negotiation, "In negotiation phase", "System");
        opportunities.Add(opp2);

        // Medium-value opportunities
        var opp3 = Opportunity.CreateNewOpportunity(
            tenantId: tenantId,
            name: "Wayne Enterprises CRM Upgrade",
            customerId: customers[0].Id, // John Doe
            value: new OpportunityValue(
                new Money(125000m, "USD"), 
                60m
            ),
            expectedCloseDate: DateTime.UtcNow.AddDays(30),
            createdBy: "System",
            description: "CRM system upgrade for Wayne Enterprises",
            type: OpportunityType.Upsell,
            priority: OpportunityPriority.Medium
        );
        opportunities.Add(opp3);

        var opp4 = Opportunity.CreateNewOpportunity(
            tenantId: tenantId,
            name: "Daily Planet Content Management",
            customerId: customers[3].Id, // Emily Davis
            value: new OpportunityValue(
                new Money(75000m, "USD"), 
                40m
            ),
            expectedCloseDate: DateTime.UtcNow.AddDays(90),
            createdBy: "System",
            description: "Content management system for Daily Planet",
            type: OpportunityType.NewBusiness,
            priority: OpportunityPriority.Medium
        );
        opportunities.Add(opp4);

        // Recently closed opportunities
        var opp5 = Opportunity.CreateNewOpportunity(
            tenantId: tenantId,
            name: "Manufacturing System Integration",
            customerId: customers[4].Id, // Robert Wilson
            value: new OpportunityValue(
                new Money(180000m, "USD"), 
                95m
            ),
            expectedCloseDate: DateTime.UtcNow.AddDays(-10),
            createdBy: "System",
            description: "Manufacturing system integration project",
            type: OpportunityType.NewBusiness,
            priority: OpportunityPriority.High
        );
        opp5.AdvanceToStage(OpportunityStage.Proposal, "Proposal accepted", "System");
        opp5.AdvanceToStage(OpportunityStage.Negotiation, "Contract negotiated", "System");
        opp5.CloseAsWon("Contract signed successfully", "System");
        opportunities.Add(opp5);

        var opp6 = Opportunity.CreateNewOpportunity(
            tenantId: tenantId,
            name: "Small Business Package",
            customerId: customers[0].Id,
            value: new OpportunityValue(
                new Money(25000m, "USD"), 
                30m
            ),
            expectedCloseDate: DateTime.UtcNow.AddDays(-5),
            createdBy: "System",
            description: "Small business ERP package",
            type: OpportunityType.NewBusiness,
            priority: OpportunityPriority.Low
        );
        opp6.CloseAsLost("Budget constraints", "System", "Price too high");
        opportunities.Add(opp6);

        await _context.Opportunities.AddRangeAsync(opportunities);
    }
}
