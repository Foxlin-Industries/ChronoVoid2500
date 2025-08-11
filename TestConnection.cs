using Npgsql;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("🔍 Testing ChronoVoid Database Connection...");
        
        var connectionString = "Host=localhost;Database=chronovoid2500;Username=postgres;Password=admin123";
        
        try
        {
            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            Console.WriteLine("✅ Database connection successful!");
            
            // Test if we can query
            var command = new NpgsqlCommand("SELECT version()", connection);
            var version = await command.ExecuteScalarAsync();
            Console.WriteLine($"✅ PostgreSQL Version: {version}");
            
            // Check if our database exists
            command = new NpgsqlCommand("SELECT current_database()", connection);
            var dbName = await command.ExecuteScalarAsync();
            Console.WriteLine($"✅ Connected to database: {dbName}");
            
            // List tables
            command = new NpgsqlCommand(@"
                SELECT table_name 
                FROM information_schema.tables 
                WHERE table_schema = 'public'
                ORDER BY table_name", connection);
            
            using var reader = await command.ExecuteReaderAsync();
            Console.WriteLine("📋 Tables in database:");
            while (await reader.ReadAsync())
            {
                Console.WriteLine($"  - {reader.GetString(0)}");
            }
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Database connection failed: {ex.Message}");
            Console.WriteLine($"   Stack trace: {ex.StackTrace}");
        }
    }
}