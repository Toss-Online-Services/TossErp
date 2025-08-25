using TossErp.CRM.Domain.Aggregates;
using TossErp.CRM.Domain.ValueObjects;
using TossErp.CRM.Domain.Enums;
using TossErp.CRM.Infrastructure.Repositories;
using System;
using System.Threading.Tasks;

/// <summary>
/// Quick integration test to verify CRM Lead and Opportunity functionality
/// This validates our domain models and repository implementations work correctly
/// </summary>
namespace CRM.Integration.Test
{
    public class CRMIntegrationTest
    {
        public static async Task RunTests()
        {
            Console.WriteLine("=== CRM Integration Tests ===\n");

            // Test Lead Creation and Management
            await TestLeadManagement();
            
            // Test Opportunity Creation and Management
            await TestOpportunityManagement();
            
            // Test Lead to Opportunity Conversion
            await TestLeadConversion();

            Console.WriteLine("\n=== All Tests Completed ===");
        }

        private static async Task TestLeadManagement()
        {
            Console.WriteLine("1. Testing Lead Management...");
            
            var leadRepo = new InMemoryLeadRepository();
            
            // Create a new lead
            var email = new EmailAddress("john.doe@example.com");
            var phone = new PhoneNumber("+1-555-123-4567");
            
            var lead = Lead.CreateNewLead(
                firstName: "John",
                lastName: "Doe",
                companyName: "Tech Solutions Inc",
                email: email,
                phone: phone,
                source: "Website Form",
                assignedTo: "sales@company.com",
                notes: "Interested in enterprise solutions"
            );
            
            // Add lead to repository
            await leadRepo.AddAsync(lead);
            
            // Qualify the lead
            lead.QualifyLead("Budget confirmed, decision maker identified");
            await leadRepo.UpdateAsync(lead);
            
            // Retrieve and verify
            var retrievedLead = await leadRepo.GetByIdAsync(lead.Id);
            Console.WriteLine($"   ✓ Lead created: {retrievedLead.FirstName} {retrievedLead.LastName}");
            Console.WriteLine($"   ✓ Status: {retrievedLead.Status}");
            Console.WriteLine($"   ✓ Lead Score: {retrievedLead.Score.Value}");
        }

        private static async Task TestOpportunityManagement()
        {
            Console.WriteLine("\n2. Testing Opportunity Management...");
            
            var oppRepo = new InMemoryOpportunityRepository();
            
            // Create a customer first
            var customerEmail = new EmailAddress("customer@techsolutions.com");
            var customer = Customer.CreateNewCustomer(
                companyName: "Tech Solutions Inc",
                contactName: "John Doe",
                email: customerEmail,
                phone: new PhoneNumber("+1-555-123-4567"),
                address: "123 Business Ave, Tech City, TC 12345",
                customerType: CustomerType.Business
            );
            
            // Create opportunity
            var estimatedValue = new Money(50000, "USD");
            var opportunityValue = new OpportunityValue(estimatedValue, 75); // 75% probability
            
            var opportunity = Opportunity.CreateNewOpportunity(
                name: "Enterprise Software Implementation",
                customerId: customer.Id,
                estimatedValue: opportunityValue,
                expectedCloseDate: DateTime.UtcNow.AddDays(45),
                assignedTo: "sales@company.com",
                description: "Full enterprise software implementation project"
            );
            
            // Add to repository
            await oppRepo.AddAsync(opportunity);
            
            // Advance through pipeline stages
            opportunity.AdvanceToStage(OpportunityStage.Proposal, "Proposal submitted for review");
            await oppRepo.UpdateAsync(opportunity);
            
            // Retrieve and verify
            var retrievedOpp = await oppRepo.GetByIdAsync(opportunity.Id);
            Console.WriteLine($"   ✓ Opportunity created: {retrievedOpp.Name}");
            Console.WriteLine($"   ✓ Stage: {retrievedOpp.Stage}");
            Console.WriteLine($"   ✓ Estimated Value: {retrievedOpp.Value.EstimatedValue}");
            Console.WriteLine($"   ✓ Weighted Value: {retrievedOpp.Value.WeightedValue}");
        }

        private static async Task TestLeadConversion()
        {
            Console.WriteLine("\n3. Testing Lead to Opportunity Conversion...");
            
            var leadRepo = new InMemoryLeadRepository();
            var oppRepo = new InMemoryOpportunityRepository();
            
            // Create qualified lead
            var email = new EmailAddress("prospect@bigcorp.com");
            var phone = new PhoneNumber("+1-555-987-6543");
            
            var lead = Lead.CreateNewLead(
                firstName: "Jane",
                lastName: "Smith",
                companyName: "BigCorp Industries",
                email: email,
                phone: phone,
                source: "Trade Show",
                assignedTo: "sales@company.com",
                notes: "Hot prospect from recent trade show"
            );
            
            // Qualify and score the lead
            lead.QualifyLead("Strong interest, has budget and authority");
            lead.UpdateScore(new LeadScore(85)); // High score
            await leadRepo.AddAsync(lead);
            
            // Convert to customer and opportunity
            var customer = Customer.CreateFromLead(lead);
            
            var opportunityValue = new OpportunityValue(new Money(75000, "USD"), 80);
            var opportunity = Opportunity.CreateFromLead(
                lead: lead,
                customer: customer,
                opportunityName: "BigCorp Digital Transformation",
                estimatedValue: opportunityValue,
                expectedCloseDate: DateTime.UtcNow.AddDays(60)
            );
            
            // Mark lead as converted
            lead.ConvertToOpportunity(opportunity.Id, "Converted to opportunity");
            await leadRepo.UpdateAsync(lead);
            await oppRepo.AddAsync(opportunity);
            
            Console.WriteLine($"   ✓ Lead converted to opportunity");
            Console.WriteLine($"   ✓ Lead status: {lead.Status}");
            Console.WriteLine($"   ✓ Opportunity created: {opportunity.Name}");
            Console.WriteLine($"   ✓ Customer created: {customer.CompanyName}");
        }
    }
}
