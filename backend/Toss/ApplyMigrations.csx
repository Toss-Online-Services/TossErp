#!/usr/bin/env dotnet-script
#r "nuget: Npgsql, 8.0.5"

using Npgsql;
using System;
using System.IO;

var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") 
    ?? "Host=localhost;Port=5432;Database=TossDb;Username=postgres;Password=postgres";

Console.WriteLine("🔄 Applying database migrations...");
Console.WriteLine($"📦 Connection: {connectionString.Replace(connectionString.Split(';').FirstOrDefault(x => x.Contains("Password"))?.Split('=')[1] ?? "", "****")}");

try
{
    await using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();
    
    Console.WriteLine("✅ Database connection successful!");
    
    // Step 1: Mark base migration as applied
    Console.WriteLine("\n📝 Step 1: Marking base migration as applied...");
    var markBaseMigration = await File.ReadAllTextAsync("MarkBaseMigrationApplied.sql");
    await using (var cmd = new NpgsqlCommand(markBaseMigration, connection))
    {
        await cmd.ExecuteNonQueryAsync();
    }
    Console.WriteLine("✅ Base migration marked as applied");
    
    // Step 2: Apply AI migration
    Console.WriteLine("\n📝 Step 2: Applying AI integration changes...");
    var aiMigration = await File.ReadAllTextAsync("AI_Migration.sql");
    await using (var cmd = new NpgsqlCommand(aiMigration, connection))
    {
        await cmd.ExecuteNonQueryAsync();
    }
    Console.WriteLine("✅ AI integration changes applied");
    
    // Verify migrations
    Console.WriteLine("\n🔍 Verifying migration history...");
    await using (var cmd = new NpgsqlCommand("SELECT \"MigrationId\" FROM \"__EFMigrationsHistory\" ORDER BY \"MigrationId\"", connection))
    {
        await using var reader = await cmd.ExecuteReaderAsync();
        Console.WriteLine("\nApplied migrations:");
        while (await reader.ReadAsync())
        {
            Console.WriteLine($"  ✓ {reader.GetString(0)}");
        }
    }
    
    Console.WriteLine("\n🎉 All migrations applied successfully!");
    Console.WriteLine("\nYou can now start the application:");
    Console.WriteLine("  dotnet run --project src/Web");
}
catch (Exception ex)
{
    Console.WriteLine($"\n❌ Error: {ex.Message}");
    Console.WriteLine("\n📋 Manual Steps:");
    Console.WriteLine("1. Connect to your PostgreSQL database");
    Console.WriteLine("2. Run: MarkBaseMigrationApplied.sql");
    Console.WriteLine("3. Run: AI_Migration.sql");
    Environment.Exit(1);
}

