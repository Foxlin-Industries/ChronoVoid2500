using Microsoft.EntityFrameworkCore;
using ChronoVoid.API.Data;
using Npgsql;

namespace ChronoVoid.Admin;

public class DatabaseTest
{
    public static async Task<bool> TestConnection()
    {
        var connectionString = "Host=localhost;Database=chronovoid2500;Username=postgres;Password=admin123";
        
        try
        {
            // Test basic connection
            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            Console.WriteLine("✅ Database connection successful!");
            
            // Test if database exists
            var command = new NpgsqlCommand("SELECT 1", connection);
            var result = await command.ExecuteScalarAsync();
            Console.WriteLine($"✅ Database query successful! Result: {result}");
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Database connection failed: {ex.Message}");
            return false;
        }
    }
    
    public static async Task<bool> TestDbContext()
    {
        var connectionString = "Host=localhost;Database=chronovoid2500;Username=postgres;Password=admin123";
        
        try
        {
            var options = new DbContextOptionsBuilder<ChronoVoidContext>()
                .UseNpgsql(connectionString)
                .Options;
                
            using var context = new ChronoVoidContext(options);
            
            // Test if we can connect and query
            var canConnect = await context.Database.CanConnectAsync();
            Console.WriteLine($"✅ DbContext can connect: {canConnect}");
            
            // Try to get count of users (should work even if table is empty)
            var userCount = await context.Users.CountAsync();
            Console.WriteLine($"✅ User count: {userCount}");
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ DbContext test failed: {ex.Message}");
            return false;
        }
    }
}