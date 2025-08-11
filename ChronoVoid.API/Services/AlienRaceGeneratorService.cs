using ChronoVoid.API.Models;

namespace ChronoVoid.API.Services;

public class AlienRaceGeneratorService
{
    private readonly Random _random;
    
    // Alien name components for procedural generation
    private static readonly string[] Prefixes = new[]
    {
        "Zar", "Keth", "Vex", "Nyx", "Qor", "Zeph", "Xal", "Mor", "Thal", "Grix",
        "Vel", "Kyr", "Nex", "Zul", "Pyx", "Rax", "Syl", "Tox", "Vyr", "Wex",
        "Aer", "Bel", "Cor", "Dex", "Fel", "Gor", "Hex", "Jex", "Kel", "Lex",
        "Myr", "Nel", "Orx", "Pex", "Qyx", "Rex", "Syx", "Tex", "Urx", "Vox"
    };
    
    private static readonly string[] Suffixes = new[]
    {
        "ani", "ori", "eki", "uli", "ari", "eni", "iri", "oni", "uri", "yri",
        "ath", "eth", "ith", "oth", "uth", "yth", "ash", "esh", "ish", "osh",
        "ush", "ysh", "ack", "eck", "ick", "ock", "uck", "yck", "ald", "eld",
        "ild", "old", "uld", "yld", "ant", "ent", "int", "ont", "unt", "ynt"
    };
    
    private static readonly string[] Connectors = new[]
    {
        "", "'", "-", " ", "al", "el", "il", "ol", "ul", "ar", "er", "ir", "or", "ur"
    };
    
    public AlienRaceGeneratorService(int? seed = null)
    {
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
    }
    
    /// <summary>
    /// Generate a single alien race with random characteristics
    /// </summary>
    public AlienRace GenerateRace()
    {
        return new AlienRace
        {
            Name = GenerateAlienName(),
            TechnologyLevel = _random.Next(1, 11), // 1-10
            TranslatorCapable = _random.NextDouble() > 0.3, // 70% chance of being translator capable
            Disposition = AlienDispositions.All[_random.Next(AlienDispositions.All.Length)],
            HumanAgreeability = GenerateWeightedAgreeability(),
            IsActive = false, // Generated races start inactive
            CreatedAt = DateTime.UtcNow,
            AdditionalTraits = GenerateAdditionalTraits()
        };
    }
    
    /// <summary>
    /// Generate multiple alien races
    /// </summary>
    public List<AlienRace> GenerateRaces(int count)
    {
        var races = new List<AlienRace>();
        var usedNames = new HashSet<string>();
        
        for (int i = 0; i < count; i++)
        {
            AlienRace race;
            int attempts = 0;
            
            // Ensure unique names
            do
            {
                race = GenerateRace();
                attempts++;
                
                // If we can't generate a unique name after 100 attempts, add a number
                if (attempts > 100)
                {
                    race.Name = $"{race.Name} {_random.Next(1000, 9999)}";
                    break;
                }
            }
            while (usedNames.Contains(race.Name));
            
            usedNames.Add(race.Name);
            races.Add(race);
        }
        
        return races;
    }
    
    /// <summary>
    /// Generate a procedural alien race name
    /// </summary>
    private string GenerateAlienName()
    {
        var nameType = _random.Next(4);
        
        return nameType switch
        {
            0 => GenerateSimpleName(),
            1 => GenerateCompoundName(),
            2 => GenerateTripleName(),
            _ => GenerateComplexName()
        };
    }
    
    private string GenerateSimpleName()
    {
        var prefix = Prefixes[_random.Next(Prefixes.Length)];
        var suffix = Suffixes[_random.Next(Suffixes.Length)];
        return $"{prefix}{suffix}";
    }
    
    private string GenerateCompoundName()
    {
        var part1 = Prefixes[_random.Next(Prefixes.Length)];
        var connector = Connectors[_random.Next(Connectors.Length)];
        var part2 = Prefixes[_random.Next(Prefixes.Length)];
        var suffix = Suffixes[_random.Next(Suffixes.Length)];
        
        return $"{part1}{connector}{part2}{suffix}";
    }
    
    private string GenerateTripleName()
    {
        var part1 = GenerateSimpleName();
        var part2 = Prefixes[_random.Next(Prefixes.Length)];
        var suffix = Suffixes[_random.Next(Suffixes.Length)];
        
        return $"{part1} {part2}{suffix}";
    }
    
    private string GenerateComplexName()
    {
        var parts = new List<string>();
        var numParts = _random.Next(2, 4);
        
        for (int i = 0; i < numParts; i++)
        {
            if (i == numParts - 1)
            {
                // Last part gets a suffix
                parts.Add($"{Prefixes[_random.Next(Prefixes.Length)]}{Suffixes[_random.Next(Suffixes.Length)]}");
            }
            else
            {
                parts.Add(Prefixes[_random.Next(Prefixes.Length)]);
            }
        }
        
        var connector = _random.NextDouble() > 0.5 ? " " : "'";
        return string.Join(connector, parts);
    }
    
    /// <summary>
    /// Generate weighted human agreeability (more races in middle range)
    /// </summary>
    private int GenerateWeightedAgreeability()
    {
        // Use normal distribution centered around 5-6
        var weights = new[] { 5, 8, 12, 15, 20, 20, 15, 12, 8, 5 }; // 1-10
        var totalWeight = weights.Sum();
        var randomValue = _random.Next(totalWeight);
        
        var currentWeight = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            currentWeight += weights[i];
            if (randomValue < currentWeight)
            {
                return i + 1; // Convert 0-based index to 1-10 range
            }
        }
        
        return 5; // Fallback to neutral
    }
    
    /// <summary>
    /// Generate additional traits as JSON
    /// </summary>
    private string GenerateAdditionalTraits()
    {
        var traits = new Dictionary<string, object>();
        
        // Physical characteristics
        var physicalTraits = new[] { "Humanoid", "Insectoid", "Reptilian", "Aquatic", "Gaseous", "Crystalline", "Mechanical", "Energy-based" };
        traits["PhysicalForm"] = physicalTraits[_random.Next(physicalTraits.Length)];
        
        // Size category
        var sizes = new[] { "Tiny", "Small", "Medium", "Large", "Huge", "Colossal" };
        traits["Size"] = sizes[_random.Next(sizes.Length)];
        
        // Lifespan
        var lifespans = new[] { "Short", "Average", "Long", "Immortal" };
        traits["Lifespan"] = lifespans[_random.Next(lifespans.Length)];
        
        // Government type
        var governments = new[] { "Democracy", "Monarchy", "Theocracy", "Military", "Anarchy", "Collective", "AI-Ruled", "Tribal" };
        traits["Government"] = governments[_random.Next(governments.Length)];
        
        // Special abilities
        if (_random.NextDouble() > 0.7) // 30% chance of special ability
        {
            var abilities = new[] { "Telepathy", "Shapeshifting", "Phase-shifting", "Regeneration", "Hive-mind", "Precognition", "Technopathy" };
            traits["SpecialAbility"] = abilities[_random.Next(abilities.Length)];
        }
        
        return System.Text.Json.JsonSerializer.Serialize(traits);
    }
    
    /// <summary>
    /// Filter races by technology level range
    /// </summary>
    public static IQueryable<AlienRace> FilterByTechLevel(IQueryable<AlienRace> races, int? minTech, int? maxTech)
    {
        if (minTech.HasValue)
            races = races.Where(r => r.TechnologyLevel >= minTech.Value);
            
        if (maxTech.HasValue)
            races = races.Where(r => r.TechnologyLevel <= maxTech.Value);
            
        return races;
    }
}