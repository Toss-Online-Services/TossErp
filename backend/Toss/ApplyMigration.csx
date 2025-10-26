#!/usr/bin/env dotnet-script
#r "nuget: Npgsql, 9.0.0"

using Npgsql;
using System;
using System.IO;

// Connection string
var connectionString = "Server=127.0.0.1;Port=5432;Database=TossErp;Username=toss;Password=toss123;";

Console.WriteLine("🔧 Applying AI Integration Migration...\n");

try
{
    await using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();
    Console.WriteLine("✅ Connected to database\n");

    // Step 1: Mark base migration as applied
    Console.WriteLine("1️⃣ Marking base migration as applied...");
    var markBaseSql = @"
        INSERT INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
        VALUES ('20251025114416_ConsolidatedEntitiesInitial', '9.0.0')
        ON CONFLICT DO NOTHING;
    ";
    
    await using (var cmd = new NpgsqlCommand(markBaseSql, connection))
    {
        await cmd.ExecuteNonQueryAsync();
        Console.WriteLine("✅ Base migration marked as applied\n");
    }

    // Step 2: Apply AI migration SQL
    Console.WriteLine("2️⃣ Applying AI integration changes...");
    var aiMigrationSql = File.ReadAllText("AI_Migration.sql");
    
    await using (var cmd = new NpgsqlCommand(aiMigrationSql, connection))
    {
        cmd.CommandTimeout = 120;
        await cmd.ExecuteNonQueryAsync();
        Console.WriteLine("✅ AI migration applied successfully\n");
    }

    // Step 3: Verify migrations
    Console.WriteLine("3️⃣ Verifying migrations...");
    var verifySql = @"SELECT ""MigrationId"" FROM ""__EFMigrationsHistory"" ORDER BY ""MigrationId""";
    await using (var cmd = new NpgsqlCommand(verifySql, connection))
    await using (var reader = await cmd.ExecuteReaderAsync())
    {
        Console.WriteLine("Applied migrations:");
        while (await reader.ReadAsync())
        {
            Console.WriteLine($"  ✅ {reader.GetString(0)}");
        }
    }

    Console.WriteLine("\n🎉 AI Integration Migration Complete!");
    Console.WriteLine("\nNext steps:");
    Console.WriteLine("  1. Configure AI provider API keys in AISettings");
    Console.WriteLine("  2. Test the AI endpoints");
    Console.WriteLine("  3. Start using AI features!\n");
}
catch (Exception ex)
{
    Console.WriteLine($"\n❌ Error: {ex.Message}");
    Console.WriteLine("\n📝 Manual alternative:");
    Console.WriteLine("  Use AI_Migration.sql with your PostgreSQL client");
    Console.WriteLine("  See APPLY_MIGRATION_INSTRUCTIONS.md for details\n");
    Environment.Exit(1);
}

