-- CRM Sample Data Seeding Script

-- Create a tenant for our sample data
\set tenant_id '12345678-1234-1234-1234-123456789abc'
\set created_by 'System'
\set now '2024-01-01 00:00:00'

-- Insert sample customers
INSERT INTO crm."Customers" ("Id", "TenantId", "Name", "Type", "Status", "Tier", "Industry", "EmployeeCount", "CreatedBy", "CreatedAt", "UpdatedBy", "UpdatedAt")
VALUES 
    ('11111111-1111-1111-1111-111111111111', :'tenant_id', 'Stark Industries', 2, 1, 3, 'Technology', 1000, :'created_by', :'now', :'created_by', :'now'),
    ('22222222-2222-2222-2222-222222222222', :'tenant_id', 'Parker Technologies', 2, 1, 2, 'Software Development', 150, :'created_by', :'now', :'created_by', :'now'),
    ('33333333-3333-3333-3333-333333333333', :'tenant_id', 'Banner Labs', 2, 1, 2, 'Research & Development', 75, :'created_by', :'now', :'created_by', :'now'),
    ('44444444-4444-4444-4444-444444444444', :'tenant_id', 'Rogers Communications', 2, 1, 3, 'Communications', 500, :'created_by', :'now', :'created_by', :'now'),
    ('55555555-5555-5555-5555-555555555555', :'tenant_id', 'Romanoff Security', 3, 2, 1, 'Cybersecurity', 25, :'created_by', :'now', :'created_by', :'now');

-- Insert sample leads
INSERT INTO crm."Leads" ("Id", "TenantId", "FirstName", "LastName", "Company", "JobTitle", "Source", "Industry", "Status", "CreatedBy", "CreatedAt", "UpdatedBy", "UpdatedAt")
VALUES 
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', :'tenant_id', 'Peter', 'Parker', 'Daily Bugle', 'Photographer', 1, 'Media', 1, :'created_by', :'now', :'created_by', :'now'),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', :'tenant_id', 'Bruce', 'Wayne', 'Wayne Industries', 'CEO', 5, 'Technology', 1, :'created_by', :'now', :'created_by', :'now'),
    ('cccccccc-cccc-cccc-cccc-cccccccccccc', :'tenant_id', 'Clark', 'Kent', 'Daily Planet', 'Journalist', 2, 'Media', 1, :'created_by', :'now', :'created_by', :'now'),
    ('dddddddd-dddd-dddd-dddd-dddddddddddd', :'tenant_id', 'Diana', 'Prince', 'Themyscira Embassy', 'Ambassador', 3, 'Government', 1, :'created_by', :'now', :'created_by', :'now'),
    ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', :'tenant_id', 'Barry', 'Allen', 'Central City Police', 'Forensic Scientist', 7, 'Law Enforcement', 1, :'created_by', :'now', :'created_by', :'now');

-- Insert sample opportunities
INSERT INTO crm."Opportunities" ("Id", "TenantId", "Name", "Stage", "EstimatedValue", "Description", "CreatedBy", "CreatedAt", "UpdatedBy", "UpdatedAt")
VALUES 
    ('xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx', :'tenant_id', 'Stark Industries Enterprise License', 2, 250000, 'Technology licensing deal', :'created_by', :'now', :'created_by', :'now'),
    ('yyyyyyyy-yyyy-yyyy-yyyy-yyyyyyyyyyyy', :'tenant_id', 'Parker Tech Development Contract', 5, 75000, 'Custom software development', :'created_by', :'now', :'created_by', :'now'),
    ('zzzzzzzz-zzzz-zzzz-zzzz-zzzzzzzzzzzz', :'tenant_id', 'Banner Labs Research Partnership', 3, 150000, 'Research collaboration agreement', :'created_by', :'now', :'created_by', :'now'),
    ('aaaabbbb-cccc-dddd-eeee-ffffffffffff', :'tenant_id', 'Rogers Communications Platform', 6, 500000, 'Communication platform deployment', :'created_by', :'now', :'created_by', :'now'),
    ('bbbbcccc-dddd-eeee-ffff-aaaaaaaaaaaa', :'tenant_id', 'Romanoff Security Consulting', 1, 35000, 'Security audit and consulting', :'created_by', :'now', :'created_by', :'now');

-- Verify the data was inserted
SELECT 'Customers' as Table, COUNT(*) as Count FROM crm."Customers"
UNION ALL
SELECT 'Leads' as Table, COUNT(*) as Count FROM crm."Leads"
UNION ALL
SELECT 'Opportunities' as Table, COUNT(*) as Count FROM crm."Opportunities";

-- Show sample data
SELECT 'Sample Customer Data:' as Info;
SELECT "Id", "Name", "Type", "Status", "Tier", "Industry", "EmployeeCount" FROM crm."Customers" LIMIT 3;

SELECT 'Sample Lead Data:' as Info;
SELECT "Id", "FirstName", "LastName", "Company", "JobTitle", "Industry" FROM crm."Leads" LIMIT 3;

SELECT 'Sample Opportunity Data:' as Info;
SELECT "Id", "Name", "Stage", "EstimatedValue", "Description" FROM crm."Opportunities" LIMIT 3;
