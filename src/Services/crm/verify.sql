-- Verify data insertion
SELECT 'Customers' as "Table_Name", COUNT(*) as "Count" FROM crm."Customers"
UNION ALL
SELECT 'Leads' as "Table_Name", COUNT(*) as "Count" FROM crm."Leads"
UNION ALL
SELECT 'Opportunities' as "Table_Name", COUNT(*) as "Count" FROM crm."Opportunities";

-- Show sample data from each table
SELECT 'Customer Sample:' as "Info";
SELECT "Id", "Name", "Type", "Status", "Industry" FROM crm."Customers" LIMIT 3;

SELECT 'Lead Sample:' as "Info";
SELECT "Id", "FirstName", "LastName", "Company", "Email" FROM crm."Leads" LIMIT 3;

SELECT 'Opportunity Sample:' as "Info";
SELECT "Id", "Name", "Stage", "EstimatedValueAmount", "Probability" FROM crm."Opportunities" LIMIT 3;
