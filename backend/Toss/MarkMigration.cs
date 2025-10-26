using System;
using System.Data;
using Npgsql;

var connectionString = "Server=127.0.0.1;Port=5432;Database=TossErp;Username=toss;Password=toss123;";
var sql = @"
INSERT INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
VALUES ('20251025114416_ConsolidatedEntitiesInitial', '9.0.0')
ON CONFLICT DO NOTHING;
";

try
{
    using var connection = new NpgsqlConnection(connectionString);
    connection.Open();
    
    using var cmd = new NpgsqlCommand(sql, connection);
    var rowsAffected = cmd.ExecuteNonQuery();
    
    Console.WriteLine($"✅ Base migration marked as applied (rows affected: {rowsAffected})");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
    Environment.Exit(1);
}

