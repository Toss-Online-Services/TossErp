using TossErp.CRM.Domain.Aggregates;
using TossErp.CRM.Domain.Enums;
using TossErp.CRM.Domain.ValueObjects;
using Crm.Domain.Entities;
using Crm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Customer = TossErp.CRM.Domain.Aggregates.Customer;
using CrmCustomerStatus = TossErp.CRM.Domain.Enums.CustomerStatus;

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
                _logger.LogInformation("CRM data already exists. Skipping seeding.");
                return;
            }

            var tenantId = Guid.NewGuid();
            var createdBy = "System";

            await SeedCustomersAsync(tenantId, createdBy);
            await SeedLeadsAsync(tenantId, createdBy);
            await SeedOpportunitiesAsync(tenantId, createdBy);

            await _context.SaveChangesAsync();
            _logger.LogInformation("CRM data seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during CRM data seeding");
            throw;
        }
    }

    private async Task SeedCustomersAsync(Guid tenantId, string createdBy)
    {
        var customers = new List<Customer>();

        var customer1 = CreateCustomerWithReflection(
            Guid.NewGuid(),
            tenantId,
            "Stark Industries",
            CustomerType.Enterprise,
            CrmCustomerStatus.Active,
            CustomerTier.Premium,
            "Technology",
            1000,
            createdBy
        );
        customers.Add(customer1);

        var customer2 = CreateCustomerWithReflection(
            Guid.NewGuid(),
            tenantId,
            "Parker Technologies",
            CustomerType.Enterprise,
            CrmCustomerStatus.Active,
            CustomerTier.Standard,
            "Software Development",
            150,
            createdBy
        );
        customers.Add(customer2);

        var customer3 = CreateCustomerWithReflection(
            Guid.NewGuid(),
            tenantId,
            "Banner Labs",
            CustomerType.Enterprise,
            CrmCustomerStatus.Active,
            CustomerTier.Standard,
            "Research & Development",
            75,
            createdBy
        );
        customers.Add(customer3);

        var customer4 = CreateCustomerWithReflection(
            Guid.NewGuid(),
            tenantId,
            "Rogers Communications",
            CustomerType.Enterprise,
            CrmCustomerStatus.Active,
            CustomerTier.Premium,
            "Communications",
            500,
            createdBy
        );
        customers.Add(customer4);

        var customer5 = CreateCustomerWithReflection(
            Guid.NewGuid(),
            tenantId,
            "Romanoff Security",
            CustomerType.SmallBusiness,
            CrmCustomerStatus.Prospect,
            CustomerTier.Basic,
            "Cybersecurity",
            25,
            createdBy
        );
        customers.Add(customer5);

        await _context.Customers.AddRangeAsync(customers);
    }

    private Customer CreateCustomerWithReflection(
        Guid id,
        Guid tenantId,
        string name,
        CustomerType type,
        CrmCustomerStatus status,
        CustomerTier tier,
        string industry,
        int employeeCount,
        string createdBy)
    {
        var customer = Activator.CreateInstance(typeof(Customer), true) as Customer;
        if (customer == null) throw new InvalidOperationException("Could not create Customer instance");

        var customerType = typeof(Customer);
        
        // Set the basic properties using reflection
        customerType.GetProperty("Id")!.SetValue(customer, id);
        customerType.GetProperty("TenantId")!.SetValue(customer, tenantId);
        customerType.GetProperty("Name")!.SetValue(customer, name);
        customerType.GetProperty("Type")!.SetValue(customer, type);
        customerType.GetProperty("Status")!.SetValue(customer, status);
        customerType.GetProperty("Tier")!.SetValue(customer, tier);
        customerType.GetProperty("Industry")!.SetValue(customer, industry);
        customerType.GetProperty("EmployeeCount")!.SetValue(customer, employeeCount);
        customerType.GetProperty("CreatedBy")!.SetValue(customer, createdBy);
        customerType.GetProperty("CreatedAt")!.SetValue(customer, DateTime.UtcNow);
        customerType.GetProperty("UpdatedBy")!.SetValue(customer, createdBy);
        customerType.GetProperty("UpdatedAt")!.SetValue(customer, DateTime.UtcNow);

        return customer;
    }

    private async Task SeedLeadsAsync(Guid tenantId, string createdBy)
    {
        var leads = new List<Lead>();

        var lead1 = CreateLeadWithReflection(
            Guid.NewGuid(),
            tenantId,
            "Peter",
            "Parker",
            "Daily Bugle",
            "Photographer",
            LeadSource.Website,
            "Media",
            createdBy
        );
        leads.Add(lead1);

        var lead2 = CreateLeadWithReflection(
            Guid.NewGuid(),
            tenantId,
            "Bruce",
            "Wayne",
            "Wayne Industries",
            "CEO",
            LeadSource.Referral,
            "Technology",
            createdBy
        );
        leads.Add(lead2);

        var lead3 = CreateLeadWithReflection(
            Guid.NewGuid(),
            tenantId,
            "Clark",
            "Kent",
            "Daily Planet",
            "Journalist",
            LeadSource.SocialMedia,
            "Media",
            createdBy
        );
        leads.Add(lead3);

        var lead4 = CreateLeadWithReflection(
            Guid.NewGuid(),
            tenantId,
            "Diana",
            "Prince",
            "Themyscira Embassy",
            "Ambassador",
            LeadSource.Email,
            "Government",
            createdBy
        );
        leads.Add(lead4);

        var lead5 = CreateLeadWithReflection(
            Guid.NewGuid(),
            tenantId,
            "Barry",
            "Allen",
            "Central City Police",
            "Forensic Scientist",
            LeadSource.TradeShow,
            "Law Enforcement",
            createdBy
        );
        leads.Add(lead5);

        await _context.Leads.AddRangeAsync(leads);
    }

    private Lead CreateLeadWithReflection(
        Guid id,
        Guid tenantId,
        string firstName,
        string lastName,
        string company,
        string jobTitle,
        LeadSource source,
        string industry,
        string createdBy)
    {
        var lead = Activator.CreateInstance(typeof(Lead), true) as Lead;
        if (lead == null) throw new InvalidOperationException("Could not create Lead instance");

        var leadType = typeof(Lead);
        
        // Set the basic properties using reflection
        leadType.GetProperty("Id")!.SetValue(lead, id);
        leadType.GetProperty("TenantId")!.SetValue(lead, tenantId);
        leadType.GetProperty("FirstName")!.SetValue(lead, firstName);
        leadType.GetProperty("LastName")!.SetValue(lead, lastName);
        leadType.GetProperty("Company")!.SetValue(lead, company);
        leadType.GetProperty("JobTitle")!.SetValue(lead, jobTitle);
        leadType.GetProperty("Source")!.SetValue(lead, source);
        leadType.GetProperty("Industry")!.SetValue(lead, industry);
        leadType.GetProperty("Status")!.SetValue(lead, LeadStatus.New);
        leadType.GetProperty("CreatedBy")!.SetValue(lead, createdBy);
        leadType.GetProperty("CreatedAt")!.SetValue(lead, DateTime.UtcNow);
        leadType.GetProperty("UpdatedBy")!.SetValue(lead, createdBy);
        leadType.GetProperty("UpdatedAt")!.SetValue(lead, DateTime.UtcNow);

        return lead;
    }

    private async Task SeedOpportunitiesAsync(Guid tenantId, string createdBy)
    {
        var opportunities = new List<Opportunity>();

        var opportunity1 = CreateOpportunityWithReflection(
            Guid.NewGuid(),
            tenantId,
            "Stark Industries Enterprise License",
            OpportunityStage.Qualification,
            250000m,
            "Technology licensing deal",
            createdBy
        );
        opportunities.Add(opportunity1);

        var opportunity2 = CreateOpportunityWithReflection(
            Guid.NewGuid(),
            tenantId,
            "Parker Tech Development Contract",
            OpportunityStage.Proposal,
            75000m,
            "Custom software development",
            createdBy
        );
        opportunities.Add(opportunity2);

        var opportunity3 = CreateOpportunityWithReflection(
            Guid.NewGuid(),
            tenantId,
            "Banner Labs Research Partnership",
            OpportunityStage.NeedsAnalysis,
            150000m,
            "Research collaboration agreement",
            createdBy
        );
        opportunities.Add(opportunity3);

        var opportunity4 = CreateOpportunityWithReflection(
            Guid.NewGuid(),
            tenantId,
            "Rogers Communications Platform",
            OpportunityStage.Negotiation,
            500000m,
            "Communication platform deployment",
            createdBy
        );
        opportunities.Add(opportunity4);

        var opportunity5 = CreateOpportunityWithReflection(
            Guid.NewGuid(),
            tenantId,
            "Romanoff Security Consulting",
            OpportunityStage.Prospecting,
            35000m,
            "Security audit and consulting",
            createdBy
        );
        opportunities.Add(opportunity5);

        await _context.Opportunities.AddRangeAsync(opportunities);
    }

    private Opportunity CreateOpportunityWithReflection(
        Guid id,
        Guid tenantId,
        string name,
        OpportunityStage stage,
        decimal estimatedValue,
        string description,
        string createdBy)
    {
        var opportunity = Activator.CreateInstance(typeof(Opportunity), true) as Opportunity;
        if (opportunity == null) throw new InvalidOperationException("Could not create Opportunity instance");

        var opportunityType = typeof(Opportunity);
        
        // Set the basic properties using reflection
        opportunityType.GetProperty("Id")!.SetValue(opportunity, id);
        opportunityType.GetProperty("TenantId")!.SetValue(opportunity, tenantId);
        opportunityType.GetProperty("Name")!.SetValue(opportunity, name);
        opportunityType.GetProperty("Stage")!.SetValue(opportunity, stage);
        opportunityType.GetProperty("EstimatedValue")!.SetValue(opportunity, estimatedValue);
        opportunityType.GetProperty("Description")!.SetValue(opportunity, description);
        opportunityType.GetProperty("CreatedBy")!.SetValue(opportunity, createdBy);
        opportunityType.GetProperty("CreatedAt")!.SetValue(opportunity, DateTime.UtcNow);
        opportunityType.GetProperty("UpdatedBy")!.SetValue(opportunity, createdBy);
        opportunityType.GetProperty("UpdatedAt")!.SetValue(opportunity, DateTime.UtcNow);

        return opportunity;
    }
}
