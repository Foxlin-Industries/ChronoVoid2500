using Npgsql;

Console.WriteLine("🔍 Testing ChronoVoid Database Connection...");

// Try different common passwords
var passwords = new[] { "admin123", "postgres", "password", "123456", "" };

foreach (var password in passwords)
{
    var connectionString = $"Host=localhost;Database=postgres;Username=postgres;Password={password}";
    Console.WriteLine($"🔍 Trying password: '{password}'");
    
    try
    {
        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        Console.WriteLine($"✅ SUCCESS! Password is: '{password}'");
        
        // Create the chronovoid2500 database if it doesn't exist
        var createDbCommand = new NpgsqlCommand("CREATE DATABASE chronovoid2500", connection);
        try
        {
            await createDbCommand.ExecuteNonQueryAsync();
            Console.WriteLine("✅ Created chronovoid2500 database");
        }
        catch (Exception createEx)
        {
            if (createEx.Message.Contains("already exists"))
            {
                Console.WriteLine("✅ chronovoid2500 database already exists");
            }
            else
            {
                Console.WriteLine($"⚠️  Could not create database: {createEx.Message}");
            }
        }
        
        // Now try to connect to our specific database
        var chronoConnectionString = $"Host=localhost;Database=chronovoid2500;Username=postgres;Password={password}";
        using var chronoConnection = new NpgsqlConnection(chronoConnectionString);
        await chronoConnection.OpenAsync();
        Console.WriteLine("✅ ChronoVoid database connection successful!");
        
        // List tables in chronovoid2500
        var tablesCommand = new NpgsqlCommand(@"
            SELECT table_name 
            FROM information_schema.tables 
            WHERE table_schema = 'public'
            ORDER BY table_name", chronoConnection);
        
        using var reader = await tablesCommand.ExecuteReaderAsync();
        Console.WriteLine("📋 Tables in chronovoid2500:");
        bool hasTables = false;
        while (await reader.ReadAsync())
        {
            Console.WriteLine($"  - {reader.GetString(0)}");
            hasTables = true;
        }
        
        if (!hasTables)
        {
            Console.WriteLine("  ⚠️  No tables found - database needs to be initialized with Entity Framework migrations");
        }
        
        return;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Failed with password '{password}': {ex.Message}");
    }
}

Console.WriteLine("❌ No working password found. Please check PostgreSQL installation and password.");
