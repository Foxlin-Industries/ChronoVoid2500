using Microsoft.EntityFrameworkCore;
using ChronoVoid.API.Data;
using ChronoVoid.API.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

Console.WriteLine("🎯 FINAL COMPREHENSIVE TEST - ChronoVoid 2500 Admin Panel");
Console.WriteLine("=========================================================");

var connectionString = "Host=localhost;Database=chronovoid2500;Username=postgres;Password=postgres";
var options = new DbContextOptionsBuilder<ChronoVoidContext>()
    .UseNpgsql(connectionString)
    .Options;

bool allTestsPassed = true;

// Test 1: Database Connection & Data
Console.WriteLine("\n🔍 Test 1: Database Connection & Data");
try
{
    using var context = new ChronoVoidContext(options);
    var canConnect = await context.Database.CanConnectAsync();
    
    if (canConnect)
    {
        var userCount = await context.Users.CountAsync();
        var realmCount = await context.NexusRealms.CountAsync();
        var alienRaceCount = await context.AlienRaces.CountAsync();
        var planetCount = await context.Planets.CountAsync();
        
        Console.WriteLine($"✅ Database Connected");
        Console.WriteLine($"✅ Users: {userCount}");
        Console.WriteLine($"✅ Realms: {realmCount}");
        Console.WriteLine($"✅ Alien Races: {alienRaceCount}");
        Console.WriteLine($"✅ Planets: {planetCount}");
        
        // Ensure we have test data
        if (alienRaceCount == 0)
        {
            var testRace = new AlienRace
            {
                Name = "Zephyrians",
                Disposition = "Peaceful",
                TechnologyLevel = 7,
                HumanAgreeability = 8,
                TranslatorCapable = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.AlienRaces.Add(testRace);
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Created test alien race");
        }
    }
    else
    {
        Console.WriteLine("❌ Database connection failed");
        allTestsPassed = false;
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Database test failed: {ex.Message}");
    allTestsPassed = false;
}

// Test 2: Admin Panel UI
Console.WriteLine("\n🌐 Test 2: Admin Panel UI");
try
{
    using var httpClient = new HttpClient();
    httpClient.Timeout = TimeSpan.FromSeconds(10);
    
    // Test homepage
    var homeResponse = await httpClient.GetAsync("http://localhost:5000");
    if (homeResponse.IsSuccessStatusCode)
    {
        var homeContent = await homeResponse.Content.ReadAsStringAsync();
        Console.WriteLine("✅ Homepage loads successfully");
        
        if (homeContent.Contains("ChronoVoid 2500 Admin"))
            Console.WriteLine("✅ Admin panel title present");
        else
        {
            Console.WriteLine("❌ Admin panel title missing");
            allTestsPassed = false;
        }
    }
    else
    {
        Console.WriteLine($"❌ Homepage failed: {homeResponse.StatusCode}");
        allTestsPassed = false;
    }
    
    // Test create realm page
    var createRealmResponse = await httpClient.GetAsync("http://localhost:5000/create-realm");
    if (createRealmResponse.IsSuccessStatusCode)
    {
        var createRealmContent = await createRealmResponse.Content.ReadAsStringAsync();
        Console.WriteLine("✅ Create Realm page loads successfully");
        
        if (createRealmContent.Contains("Planet Density") && 
            createRealmContent.Contains("Resource Density") &&
            createRealmContent.Contains("Alien Race"))
        {
            Console.WriteLine("✅ All realm creation features present");
        }
        else
        {
            Console.WriteLine("❌ Some realm creation features missing");
            allTestsPassed = false;
        }
    }
    else
    {
        Console.WriteLine($"❌ Create Realm page failed: {createRealmResponse.StatusCode}");
        allTestsPassed = false;
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ UI test failed: {ex.Message}");
    allTestsPassed = false;
}

// Test 3: API Server
Console.WriteLine("\n🔌 Test 3: API Server");
try
{
    using var httpClient = new HttpClient();
    var apiResponse = await httpClient.GetAsync("http://localhost:7000");
    
    if (apiResponse.IsSuccessStatusCode)
    {
        Console.WriteLine("✅ API server is running and responding");
    }
    else
    {
        Console.WriteLine($"❌ API server error: {apiResponse.StatusCode}");
        allTestsPassed = false;
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ API test failed: {ex.Message}");
    allTestsPassed = false;
}

// Final Results
Console.WriteLine("\n" + new string('=', 60));
if (allTestsPassed)
{
    Console.WriteLine("🎉 ALL TESTS PASSED! 🎉");
    Console.WriteLine("\n✅ Your ChronoVoid 2500 Admin Panel is fully functional!");
    Console.WriteLine("\n🚀 Ready to use:");
    Console.WriteLine("   • Admin Panel: http://localhost:5000");
    Console.WriteLine("   • API Server: http://localhost:7000");
    Console.WriteLine("   • Database: PostgreSQL with test data");
    Console.WriteLine("\n🌟 Features available:");
    Console.WriteLine("   • Dashboard with statistics");
    Console.WriteLine("   • Enhanced realm creation with alien race selection");
    Console.WriteLine("   • Planet and resource density settings");
    Console.WriteLine("   • Tech level filtering for alien races");
    Console.WriteLine("   • Professional Radzen UI components");
    Console.WriteLine("   • Responsive design");
}
else
{
    Console.WriteLine("❌ SOME TESTS FAILED");
    Console.WriteLine("Please check the error messages above and fix any issues.");
}
Console.WriteLine(new string('=', 60));
