using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Text.Json;

namespace DirectDataSeeder;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting direct data seeding test...");

        var connectionString = "Host=localhost;Port=5432;Database=tosserpdb;Username=postgres;Password=postgres123;";

        try
        {
            // Test connection first
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            Console.WriteLine("Database connection successful!");

            // Check if customers already exist
            await using var checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM crm.\"Customers\"", connection);
            var count = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());
            
            if (count > 0)
            {
                Console.WriteLine($"Found {count} existing customers. Skipping seeding.");
                return;
            }

            Console.WriteLine("No existing customers found. Starting data seeding...");

            // Insert customers using simple SQL
            var insertCustomerSql = @"
                INSERT INTO crm.""Customers"" (""Id"", ""TenantId"", ""Name"", ""Type"", ""Status"", ""Tier"", ""Industry"", ""EmployeeCount"", ""CreatedBy"", ""CreatedAt"", ""UpdatedBy"", ""UpdatedAt"")
                VALUES (@Id, @TenantId, @Name, @Type, @Status, @Tier, @Industry, @EmployeeCount, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt)";

            var tenantId = Guid.NewGuid();
            var createdBy = "System";
            var now = DateTime.UtcNow;

            var customers = new[]
            {
                new { Id = Guid.NewGuid(), Name = "Stark Industries", Type = 2, Status = 1, Tier = 3, Industry = "Technology", EmployeeCount = 1000 },
                new { Id = Guid.NewGuid(), Name = "Parker Technologies", Type = 2, Status = 1, Tier = 2, Industry = "Software Development", EmployeeCount = 150 },
                new { Id = Guid.NewGuid(), Name = "Banner Labs", Type = 2, Status = 1, Tier = 2, Industry = "Research & Development", EmployeeCount = 75 },
                new { Id = Guid.NewGuid(), Name = "Rogers Communications", Type = 2, Status = 1, Tier = 3, Industry = "Communications", EmployeeCount = 500 },
                new { Id = Guid.NewGuid(), Name = "Romanoff Security", Type = 3, Status = 2, Tier = 1, Industry = "Cybersecurity", EmployeeCount = 25 }
            };

            foreach (var customer in customers)
            {
                await using var cmd = new NpgsqlCommand(insertCustomerSql, connection);
                cmd.Parameters.AddWithValue("@Id", customer.Id);
                cmd.Parameters.AddWithValue("@TenantId", tenantId);
                cmd.Parameters.AddWithValue("@Name", customer.Name);
                cmd.Parameters.AddWithValue("@Type", customer.Type);
                cmd.Parameters.AddWithValue("@Status", customer.Status);
                cmd.Parameters.AddWithValue("@Tier", customer.Tier);
                cmd.Parameters.AddWithValue("@Industry", customer.Industry);
                cmd.Parameters.AddWithValue("@EmployeeCount", customer.EmployeeCount);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                cmd.Parameters.AddWithValue("@CreatedAt", now);
                cmd.Parameters.AddWithValue("@UpdatedBy", createdBy);
                cmd.Parameters.AddWithValue("@UpdatedAt", now);

                await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"Inserted customer: {customer.Name}");
            }

            // Insert leads
            var insertLeadSql = @"
                INSERT INTO crm.""Leads"" (""Id"", ""TenantId"", ""FirstName"", ""LastName"", ""Company"", ""JobTitle"", ""Source"", ""Industry"", ""Status"", ""CreatedBy"", ""CreatedAt"", ""UpdatedBy"", ""UpdatedAt"")
                VALUES (@Id, @TenantId, @FirstName, @LastName, @Company, @JobTitle, @Source, @Industry, @Status, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt)";

            var leads = new[]
            {
                new { Id = Guid.NewGuid(), FirstName = "Peter", LastName = "Parker", Company = "Daily Bugle", JobTitle = "Photographer", Source = 1, Industry = "Media", Status = 1 },
                new { Id = Guid.NewGuid(), FirstName = "Bruce", LastName = "Wayne", Company = "Wayne Industries", JobTitle = "CEO", Source = 5, Industry = "Technology", Status = 1 },
                new { Id = Guid.NewGuid(), FirstName = "Clark", LastName = "Kent", Company = "Daily Planet", JobTitle = "Journalist", Source = 2, Industry = "Media", Status = 1 },
                new { Id = Guid.NewGuid(), FirstName = "Diana", LastName = "Prince", Company = "Themyscira Embassy", JobTitle = "Ambassador", Source = 3, Industry = "Government", Status = 1 },
                new { Id = Guid.NewGuid(), FirstName = "Barry", LastName = "Allen", Company = "Central City Police", JobTitle = "Forensic Scientist", Source = 7, Industry = "Law Enforcement", Status = 1 }
            };

            foreach (var lead in leads)
            {
                await using var cmd = new NpgsqlCommand(insertLeadSql, connection);
                cmd.Parameters.AddWithValue("@Id", lead.Id);
                cmd.Parameters.AddWithValue("@TenantId", tenantId);
                cmd.Parameters.AddWithValue("@FirstName", lead.FirstName);
                cmd.Parameters.AddWithValue("@LastName", lead.LastName);
                cmd.Parameters.AddWithValue("@Company", lead.Company);
                cmd.Parameters.AddWithValue("@JobTitle", lead.JobTitle);
                cmd.Parameters.AddWithValue("@Source", lead.Source);
                cmd.Parameters.AddWithValue("@Industry", lead.Industry);
                cmd.Parameters.AddWithValue("@Status", lead.Status);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                cmd.Parameters.AddWithValue("@CreatedAt", now);
                cmd.Parameters.AddWithValue("@UpdatedBy", createdBy);
                cmd.Parameters.AddWithValue("@UpdatedAt", now);

                await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"Inserted lead: {lead.FirstName} {lead.LastName}");
            }

            // Insert opportunities
            var insertOpportunitySql = @"
                INSERT INTO crm.""Opportunities"" (""Id"", ""TenantId"", ""Name"", ""Stage"", ""EstimatedValue"", ""Description"", ""CreatedBy"", ""CreatedAt"", ""UpdatedBy"", ""UpdatedAt"")
                VALUES (@Id, @TenantId, @Name, @Stage, @EstimatedValue, @Description, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt)";

            var opportunities = new[]
            {
                new { Id = Guid.NewGuid(), Name = "Stark Industries Enterprise License", Stage = 2, EstimatedValue = 250000m, Description = "Technology licensing deal" },
                new { Id = Guid.NewGuid(), Name = "Parker Tech Development Contract", Stage = 5, EstimatedValue = 75000m, Description = "Custom software development" },
                new { Id = Guid.NewGuid(), Name = "Banner Labs Research Partnership", Stage = 3, EstimatedValue = 150000m, Description = "Research collaboration agreement" },
                new { Id = Guid.NewGuid(), Name = "Rogers Communications Platform", Stage = 6, EstimatedValue = 500000m, Description = "Communication platform deployment" },
                new { Id = Guid.NewGuid(), Name = "Romanoff Security Consulting", Stage = 1, EstimatedValue = 35000m, Description = "Security audit and consulting" }
            };

            foreach (var opportunity in opportunities)
            {
                await using var cmd = new NpgsqlCommand(insertOpportunitySql, connection);
                cmd.Parameters.AddWithValue("@Id", opportunity.Id);
                cmd.Parameters.AddWithValue("@TenantId", tenantId);
                cmd.Parameters.AddWithValue("@Name", opportunity.Name);
                cmd.Parameters.AddWithValue("@Stage", opportunity.Stage);
                cmd.Parameters.AddWithValue("@EstimatedValue", opportunity.EstimatedValue);
                cmd.Parameters.AddWithValue("@Description", opportunity.Description);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                cmd.Parameters.AddWithValue("@CreatedAt", now);
                cmd.Parameters.AddWithValue("@UpdatedBy", createdBy);
                cmd.Parameters.AddWithValue("@UpdatedAt", now);

                await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"Inserted opportunity: {opportunity.Name}");
            }

            Console.WriteLine("Data seeding completed successfully!");

            // Verify the inserted data
            await using var verifyCmd = new NpgsqlCommand(@"
                SELECT 
                    (SELECT COUNT(*) FROM crm.""Customers"") as CustomerCount,
                    (SELECT COUNT(*) FROM crm.""Leads"") as LeadCount,
                    (SELECT COUNT(*) FROM crm.""Opportunities"") as OpportunityCount",
                connection);

            await using var reader = await verifyCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                Console.WriteLine($"Final counts: {reader["CustomerCount"]} customers, {reader["LeadCount"]} leads, {reader["OpportunityCount"]} opportunities");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during data seeding: {ex.Message}");
            throw;
        }
    }
}
