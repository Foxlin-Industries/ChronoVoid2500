using System.Net;
using System.Net.Http.Json;
using ChronoVoid2500.UnitTests.TestInfrastructure;
using FluentAssertions;

namespace ChronoVoid2500.UnitTests.Api;

public class AlienRaceTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public AlienRaceTests(ApiTestFixture fx)
    {
        _client = fx.Client;
    }

    [Fact]
    public async Task Get_Alien_Races_Should_Return_List_Of_Races()
    {
        var response = await _client.GetAsync("/api/alienrace");
        response.EnsureSuccessStatusCode();
        
        var races = await response.Content.ReadFromJsonAsync<List<AlienRaceDto>>();
        races.Should().NotBeNull();
        races!.Count.Should().BeGreaterThanOrEqualTo(0);
        
        if (races.Count > 0)
        {
            var race = races[0];
            race.Id.Should().BeGreaterThan(0);
            race.Name.Should().NotBeNullOrEmpty();
            race.TechnologyLevel.Should().BeInRange(1, 10);
            // TranslatorCapable is a boolean value (no assertion needed)
        }
    }

    [Fact]
    public async Task Get_Alien_Race_By_Id_Should_Return_Valid_Race()
    {
        // Get a list of races first
        var racesResponse = await _client.GetAsync("/api/alienrace");
        racesResponse.EnsureSuccessStatusCode();
        var races = await racesResponse.Content.ReadFromJsonAsync<List<AlienRaceDto>>();
        races.Should().NotBeNull();
        races!.Count.Should().BeGreaterThan(0);

        var raceId = races[0].Id;
        var response = await _client.GetAsync($"/api/alienrace/{raceId}");
        response.EnsureSuccessStatusCode();
        
        var race = await response.Content.ReadFromJsonAsync<AlienRaceDto>();
        race.Should().NotBeNull();
        race!.Id.Should().Be(raceId);
        race.Name.Should().NotBeNullOrEmpty();
        race.TechnologyLevel.Should().BeInRange(1, 10);
        // TranslatorCapable is a boolean value (no assertion needed)
    }

    [Fact]
    public async Task Get_Non_Existent_Alien_Race_Should_Return_NotFound()
    {
        var response = await _client.GetAsync("/api/alienrace/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Generate_Alien_Races_Should_Create_New_Races()
    {
        var generateRequest = new { Count = 5 };
        var response = await _client.PostAsJsonAsync("/api/alienrace/generate", generateRequest);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<AlienRaceGenerationResultDto>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.GeneratedCount.Should().Be(5);
        result.Message.Should().Contain("generated successfully");
    }

    [Fact]
    public async Task Generate_Alien_Races_With_Invalid_Count_Should_Fail()
    {
        var generateRequest = new { Count = 0 }; // Invalid count
        var response = await _client.PostAsJsonAsync("/api/alienrace/generate", generateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Generate_Alien_Races_With_Too_Large_Count_Should_Fail()
    {
        var generateRequest = new { Count = 10001 }; // Too many
        var response = await _client.PostAsJsonAsync("/api/alienrace/generate", generateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_Alien_Races_By_Technology_Level_Should_Filter_Correctly()
    {
        // Generate some races first
        var generateRequest = new { Count = 10 };
        var generateResponse = await _client.PostAsJsonAsync("/api/alienrace/generate", generateRequest);
        generateResponse.EnsureSuccessStatusCode();

        // Get races with technology level 5
        var response = await _client.GetAsync("/api/alienrace?technologyLevel=5");
        response.EnsureSuccessStatusCode();
        
        var races = await response.Content.ReadFromJsonAsync<List<AlienRaceDto>>();
        races.Should().NotBeNull();
        
        // All returned races should have technology level 5
        foreach (var race in races!)
        {
            race.TechnologyLevel.Should().Be(5);
        }
    }

    [Fact]
    public async Task Get_Translator_Capable_Alien_Races_Should_Filter_Correctly()
    {
        var response = await _client.GetAsync("/api/alienrace?translatorCapable=true");
        response.EnsureSuccessStatusCode();
        
        var races = await response.Content.ReadFromJsonAsync<List<AlienRaceDto>>();
        races.Should().NotBeNull();
        
        // All returned races should be translator capable
        foreach (var race in races!)
        {
            race.TranslatorCapable.Should().BeTrue();
        }
    }

    [Fact]
    public async Task Get_Alien_Races_With_Multiple_Filters_Should_Work()
    {
        var response = await _client.GetAsync("/api/alienrace?technologyLevel=7&translatorCapable=true");
        response.EnsureSuccessStatusCode();
        
        var races = await response.Content.ReadFromJsonAsync<List<AlienRaceDto>>();
        races.Should().NotBeNull();
        
        // All returned races should match both criteria
        foreach (var race in races!)
        {
            race.TechnologyLevel.Should().Be(7);
            race.TranslatorCapable.Should().BeTrue();
        }
    }

    [Fact]
    public async Task Delete_Alien_Race_Should_Remove_From_Database()
    {
        // Generate a race first
        var generateRequest = new { Count = 1 };
        var generateResponse = await _client.PostAsJsonAsync("/api/alienrace/generate", generateRequest);
        generateResponse.EnsureSuccessStatusCode();
        
        var generateResult = await generateResponse.Content.ReadFromJsonAsync<AlienRaceGenerationResultDto>();
        generateResult.Should().NotBeNull();

        // Get the generated race
        var racesResponse = await _client.GetAsync("/api/alienrace");
        racesResponse.EnsureSuccessStatusCode();
        var races = await racesResponse.Content.ReadFromJsonAsync<List<AlienRaceDto>>();
        races.Should().NotBeNull();
        races!.Count.Should().BeGreaterThan(0);

        var raceToDelete = races.Last(); // Get the most recently generated race

        // Delete the race
        var deleteResponse = await _client.DeleteAsync($"/api/alienrace/{raceToDelete.Id}");
        deleteResponse.EnsureSuccessStatusCode();

        // Verify the race is deleted
        var getResponse = await _client.GetAsync($"/api/alienrace/{raceToDelete.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Non_Existent_Alien_Race_Should_Return_NotFound()
    {
        var response = await _client.DeleteAsync("/api/alienrace/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_Alien_Race_Should_Modify_Properties()
    {
        // Generate a race first
        var generateRequest = new { Count = 1 };
        var generateResponse = await _client.PostAsJsonAsync("/api/alienrace/generate", generateRequest);
        generateResponse.EnsureSuccessStatusCode();

        // Get the generated race
        var racesResponse = await _client.GetAsync("/api/alienrace");
        racesResponse.EnsureSuccessStatusCode();
        var races = await racesResponse.Content.ReadFromJsonAsync<List<AlienRaceDto>>();
        races.Should().NotBeNull();
        races!.Count.Should().BeGreaterThan(0);

        var raceToUpdate = races.Last();
        var updateRequest = new AlienRaceUpdateDto
        {
            Name = "Updated Race Name",
            TechnologyLevel = 8,
            TranslatorCapable = true
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/alienrace/{raceToUpdate.Id}", updateRequest);
        updateResponse.EnsureSuccessStatusCode();

        // Verify the update
        var getResponse = await _client.GetAsync($"/api/alienrace/{raceToUpdate.Id}");
        getResponse.EnsureSuccessStatusCode();
        var updatedRace = await getResponse.Content.ReadFromJsonAsync<AlienRaceDto>();
        
        updatedRace.Should().NotBeNull();
        updatedRace!.Name.Should().Be("Updated Race Name");
        updatedRace.TechnologyLevel.Should().Be(8);
        updatedRace.TranslatorCapable.Should().BeTrue();
    }

    [Fact]
    public async Task Update_Alien_Race_With_Invalid_Data_Should_Fail()
    {
        // Generate a race first
        var generateRequest = new { Count = 1 };
        var generateResponse = await _client.PostAsJsonAsync("/api/alienrace/generate", generateRequest);
        generateResponse.EnsureSuccessStatusCode();

        // Get the generated race
        var racesResponse = await _client.GetAsync("/api/alienrace");
        racesResponse.EnsureSuccessStatusCode();
        var races = await racesResponse.Content.ReadFromJsonAsync<List<AlienRaceDto>>();
        races.Should().NotBeNull();
        races!.Count.Should().BeGreaterThan(0);

        var raceToUpdate = races.Last();
        var updateRequest = new AlienRaceUpdateDto
        {
            Name = "", // Invalid empty name
            TechnologyLevel = 15, // Invalid technology level
            TranslatorCapable = true
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/alienrace/{raceToUpdate.Id}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // DTOs for the tests
    private record AlienRaceDto(int Id, string Name, int TechnologyLevel, bool TranslatorCapable, DateTime CreatedAt);
    private record AlienRaceGenerationResultDto(bool Success, int GeneratedCount, string Message);
    private record AlienRaceUpdateDto
    {
        public string Name { get; init; } = string.Empty;
        public int TechnologyLevel { get; init; }
        public bool TranslatorCapable { get; init; }
    }
}
