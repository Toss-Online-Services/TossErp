#r "nuget: Npgsql, 8.0.0"
#r "nuget: System.Data.Common, 4.3.0"

using System;
using System.Data;
using Npgsql;

var connectionString = "Server=127.0.0.1;Port=5432;Database=TossErp;Username=toss;Password=toss123;";

Console.WriteLine("🔍 Connecting to database...");
Console.WriteLine("Database: TossErp");
Console.WriteLine("Server: 127.0.0.1:5432");
Console.WriteLine("");

using var connection = new NpgsqlConnection(connectionString);
connection.Open();

Console.WriteLine("✅ Connected to database!");
Console.WriteLine("");

// Get total count
var countQuery = "SELECT COUNT(*) FROM \"SalesDocuments\" WHERE \"ShopId\" = 1;";
using var countCmd = new NpgsqlCommand(countQuery, connection);
var totalCount = countCmd.ExecuteScalar();
Console.WriteLine($"📊 Total SalesDocuments for ShopId = 1: {totalCount}");
Console.WriteLine("");

// Get breakdown by DocumentType
var breakdownQuery = @"
    SELECT 
        \"DocumentType\",
        COUNT(*) as Count
    FROM \"SalesDocuments\"
    WHERE \"ShopId\" = 1
    GROUP BY \"DocumentType\"
    ORDER BY \"DocumentType\";
";

Console.WriteLine("📋 Breakdown by DocumentType:");
Console.WriteLine("─────────────────────────────────────");
using var breakdownCmd = new NpgsqlCommand(breakdownQuery, connection);
using var reader = breakdownCmd.ExecuteReader();
while (reader.Read())
{
    var docType = reader.GetInt32(0);
    var count = reader.GetInt64(1);
    var typeName = docType == 1 ? "Invoice" : docType == 2 ? "Receipt" : "Unknown";
    Console.WriteLine($"  {typeName} (Type {docType}): {count}");
}
reader.Close();
Console.WriteLine("");

// Get all documents with details
var detailsQuery = @"
    SELECT 
        \"Id\",
        \"DocumentNumber\",
        \"DocumentType\",
        \"CustomerId\",
        \"DocumentDate\",
        \"TotalAmount\",
        \"SaleId\",
        \"IsPaid\"
    FROM \"SalesDocuments\"
    WHERE \"ShopId\" = 1
    ORDER BY \"DocumentDate\" DESC
    LIMIT 20;
";

Console.WriteLine("📄 All SalesDocuments (first 20):");
Console.WriteLine("─────────────────────────────────────────────────────────────────────────────────────");
using var detailsCmd = new NpgsqlCommand(detailsQuery, connection);
using var detailsReader = detailsCmd.ExecuteReader();
int rowNum = 1;
while (detailsReader.Read())
{
    var id = detailsReader.GetInt32(0);
    var docNumber = detailsReader.IsDBNull(1) ? "N/A" : detailsReader.GetString(1);
    var docType = detailsReader.GetInt32(2);
    var customerId = detailsReader.IsDBNull(3) ? (int?)null : detailsReader.GetInt32(3);
    var docDate = detailsReader.GetDateTime(4);
    var totalAmount = detailsReader.IsDBNull(5) ? 0m : detailsReader.GetDecimal(5);
    var saleId = detailsReader.IsDBNull(6) ? (int?)null : detailsReader.GetInt32(6);
    var isPaid = detailsReader.IsDBNull(7) ? false : detailsReader.GetBoolean(7);
    
    var typeName = docType == 1 ? "INV" : docType == 2 ? "RCT" : "UNK";
    
    Console.WriteLine($"{rowNum:D2}. [{typeName}] {docNumber} | CustomerId: {customerId?.ToString() ?? "N/A"} | SaleId: {saleId?.ToString() ?? "N/A"} | Date: {docDate:yyyy-MM-dd} | Total: R{totalAmount:F2} | Paid: {isPaid}");
    rowNum++;
}
detailsReader.Close();

Console.WriteLine("");
Console.WriteLine("✅ Query complete!");











