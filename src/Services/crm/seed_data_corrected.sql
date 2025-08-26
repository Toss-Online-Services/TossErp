-- CRM Sample Data Seeding Script (Corrected)

-- Create a tenant for our sample data
\set tenant_id '12345678-1234-1234-1234-123456789abc'
\set created_by 'System'
\set now '2024-01-01 00:00:00'

-- Insert sample customers
INSERT INTO crm."Customers" ("Id", "TenantId", "Name", "Type", "Status", "Tier", "Industry", "EmployeeCount", "CreatedBy", "CreatedAt", "ModifiedBy", "ModifiedAt", "SubscriptionStatus")
VALUES 
    ('11111111-1111-1111-1111-111111111111', :'tenant_id', 'Stark Industries', 2, 1, 3, 'Technology', 1000, :'created_by', :'now', :'created_by', :'now', 1),
    ('22222222-2222-2222-2222-222222222222', :'tenant_id', 'Parker Technologies', 2, 1, 2, 'Software Development', 150, :'created_by', :'now', :'created_by', :'now', 1),
    ('33333333-3333-3333-3333-333333333333', :'tenant_id', 'Banner Labs', 2, 1, 2, 'Research & Development', 75, :'created_by', :'now', :'created_by', :'now', 1),
    ('44444444-4444-4444-4444-444444444444', :'tenant_id', 'Rogers Communications', 2, 1, 3, 'Communications', 500, :'created_by', :'now', :'created_by', :'now', 1),
    ('55555555-5555-5555-5555-555555555555', :'tenant_id', 'Romanoff Security', 3, 2, 1, 'Cybersecurity', 25, :'created_by', :'now', :'created_by', :'now', 2);

-- Insert sample leads
INSERT INTO crm."Leads" ("Id", "TenantId", "FirstName", "LastName", "Company", "Email", "JobTitle", "Source", "Industry", "Status", "Score", "CreatedBy", "CreatedAt", "ModifiedBy", "ModifiedAt", "ContactAttempts", "IsDeleted")
VALUES 
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', :'tenant_id', 'Peter', 'Parker', 'Daily Bugle', 'peter@dailybugle.com', 'Photographer', 'Website', 'Media', 'New', 75, :'created_by', :'now', :'created_by', :'now', 0, false),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', :'tenant_id', 'Bruce', 'Wayne', 'Wayne Industries', 'bruce@wayneind.com', 'CEO', 'Referral', 'Technology', 'New', 90, :'created_by', :'now', :'created_by', :'now', 0, false),
    ('cccccccc-cccc-cccc-cccc-cccccccccccc', :'tenant_id', 'Clark', 'Kent', 'Daily Planet', 'clark@dailyplanet.com', 'Journalist', 'SocialMedia', 'Media', 'New', 85, :'created_by', :'now', :'created_by', :'now', 0, false),
    ('dddddddd-dddd-dddd-dddd-dddddddddddd', :'tenant_id', 'Diana', 'Prince', 'Themyscira Embassy', 'diana@themyscira.gov', 'Ambassador', 'Email', 'Government', 'New', 95, :'created_by', :'now', :'created_by', :'now', 0, false),
    ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', :'tenant_id', 'Barry', 'Allen', 'Central City Police', 'barry@ccpd.gov', 'Forensic Scientist', 'TradeShow', 'Law Enforcement', 'New', 80, :'created_by', :'now', :'created_by', :'now', 0, false);

-- Insert sample opportunities
INSERT INTO crm."Opportunities" ("Id", "TenantId", "Name", "Stage", "EstimatedValueAmount", "EstimatedValueCurrency", "Description", "CreatedBy", "CreatedAt", "ModifiedBy", "ModifiedAt")
VALUES 
    ('ffffffff-ffff-ffff-ffff-ffffffffffff', :'tenant_id', 'Stark Industries Enterprise License', 'Qualification', 250000, 'USD', 'Technology licensing deal', :'created_by', :'now', :'created_by', :'now'),
    ('gggggggg-gggg-gggg-gggg-gggggggggggg', :'tenant_id', 'Parker Tech Development Contract', 'Proposal', 75000, 'USD', 'Custom software development', :'created_by', :'now', :'created_by', :'now'),
    ('hhhhhhhh-hhhh-hhhh-hhhh-hhhhhhhhhhhh', :'tenant_id', 'Banner Labs Research Partnership', 'NeedsAnalysis', 150000, 'USD', 'Research collaboration agreement', :'created_by', :'now', :'created_by', :'now'),
    ('iiiiiiii-iiii-iiii-iiii-iiiiiiiiiiii', :'tenant_id', 'Rogers Communications Platform', 'Negotiation', 500000, 'USD', 'Communication platform deployment', :'created_by', :'now', :'created_by', :'now'),
    ('jjjjjjjj-jjjj-jjjj-jjjj-jjjjjjjjjjjj', :'tenant_id', 'Romanoff Security Consulting', 'Prospecting', 35000, 'USD', 'Security audit and consulting', :'created_by', :'now', :'created_by', :'now');

-- Verify the data was inserted
SELECT 'Customers' as "Table", COUNT(*) as "Count" FROM crm."Customers"
UNION ALL
SELECT 'Leads' as "Table", COUNT(*) as "Count" FROM crm."Leads"
UNION ALL
SELECT 'Opportunities' as "Table", COUNT(*) as "Count" FROM crm."Opportunities";

-- Show sample data
SELECT 'Sample Customer Data:' as "Info";
SELECT "Id", "Name", "Type", "Status", "Tier", "Industry", "EmployeeCount" FROM crm."Customers" LIMIT 3;

SELECT 'Sample Lead Data:' as "Info";
SELECT "Id", "FirstName", "LastName", "Company", "JobTitle", "Industry" FROM crm."Leads" LIMIT 3;

SELECT 'Sample Opportunity Data:' as "Info";
SELECT "Id", "Name", "Stage", "EstimatedValueAmount", "Description" FROM crm."Opportunities" LIMIT 3;
