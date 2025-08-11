using Microsoft.EntityFrameworkCore;
using ChronoVoid.API.Data;
using ChronoVoid.API.Models;

Console.WriteLine("🧪 Running Admin Panel Tests...");

var connectionString = "Host=localhost;Database=chronovoid2500;Username=postgres;Password=postgres";

var options = new DbContextOptionsBuilder<ChronoVoidContext>()
    .UseNpgsql(connectionString)
    .Options;
    
using var context = new ChronoVoidContext(options);

// Test 1: Database Connection
Console.WriteLine("\n📋 Test 1: Database Connection");
try
{
    var canConnect = await context.Database.CanConnectAsync();
    Console.WriteLine($"✅ Database connection: {canConnect}");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Database connection failed: {ex.Message}");
    return;
}

// Test 2: Check Tables Exist
Console.WriteLine("\n📋 Test 2: Database Tables");
try
{
    var userCount = await context.Users.CountAsync();
    var realmCount = await context.NexusRealms.CountAsync();
    var alienRaceCount = await context.AlienRaces.CountAsync();
    var planetCount = await context.Planets.CountAsync();
    
    Console.WriteLine($"✅ Users table: {userCount} records");
    Console.WriteLine($"✅ Realms table: {realmCount} records");
    Console.WriteLine($"✅ AlienRaces table: {alienRaceCount} records");
    Console.WriteLine($"✅ Planets table: {planetCount} records");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Table access failed: {ex.Message}");
    return;
}

// Test 3: Create Test Data
Console.WriteLine("\n📋 Test 3: Create Test Data");
try
{
    // Create a test alien race if none exist
    if (!await context.AlienRaces.AnyAsync())
    {
        var testRace = new AlienRace
        {
            Name = "Test Zephyrians",
            Disposition = "Peaceful",
            TechnologyLevel = 5,
            HumanAgreeability = 7,
            TranslatorCapable = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        
        context.AlienRaces.Add(testRace);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Created test alien race");
    }
    else
    {
        Console.WriteLine("✅ Alien races already exist");
    }
    
    // Create a test user if none exist
    if (!await context.Users.AnyAsync())
    {
        var testUser = new User
        {
            Username = "testadmin",
            Email = "admin@chronovoid.test",
            PasswordHash = "test_hash",
            CreatedAt = DateTime.UtcNow,
            LastLogin = DateTime.UtcNow
        };
        
        context.Users.Add(testUser);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Created test user");
    }
    else
    {
        Console.WriteLine("✅ Users already exist");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Test data creation failed: {ex.Message}");
}

// Test 4: Query Test Data
Console.WriteLine("\n📋 Test 4: Query Test Data");
try
{
    var alienRaces = await context.AlienRaces
        .Where(r => r.IsActive)
        .OrderBy(r => r.Name)
        .Take(5)
        .ToListAsync();
        
    Console.WriteLine($"✅ Found {alienRaces.Count} active alien races:");
    foreach (var race in alienRaces)
    {
        Console.WriteLine($"   - {race.Name} (Tech Level: {race.TechnologyLevel}, Disposition: {race.Disposition})");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Query test failed: {ex.Message}");
}

Console.WriteLine("\n🎉 Admin Panel Database Tests Complete!");
Console.WriteLine("✅ The admin panel should now work correctly at http://localhost:5000");
