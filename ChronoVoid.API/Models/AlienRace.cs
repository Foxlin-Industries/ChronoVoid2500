using System.ComponentModel.DataAnnotations;

namespace ChronoVoid.API.Models;

public class AlienRace
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Technology level from 1-10 (1=primitive cave dwellers, 10=human-level advanced)
    /// </summary>
    [Range(1, 10)]
    public int TechnologyLevel { get; set; }
    
    /// <summary>
    /// Can humans communicate with this race using translators?
    /// </summary>
    public bool TranslatorCapable { get; set; }
    
    /// <summary>
    /// General disposition/behavior type of this race
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Disposition { get; set; } = string.Empty;
    
    /// <summary>
    /// How agreeable this race is with humans (1=hostile, 10=best friends)
    /// </summary>
    [Range(1, 10)]
    public int HumanAgreeability { get; set; }
    
    /// <summary>
    /// Is this race currently active in the game world?
    /// </summary>
    public bool IsActive { get; set; } = false;
    
    /// <summary>
    /// When this race was generated
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Additional characteristics as JSON
    /// </summary>
    public string? AdditionalTraits { get; set; }
}

/// <summary>
/// Static list of possible alien race dispositions
/// </summary>
public static class AlienDispositions
{
    public static readonly string[] All = new[]
    {
        "Peaceful",
        "Aggressive", 
        "Traders",
        "Warriors",
        "Scholars",
        "Explorers",
        "Isolationists",
        "Expansionists",
        "Mercenaries",
        "Pirates",
        "Diplomats",
        "Xenophobic",
        "Xenophilic",
        "Nomadic",
        "Territorial",
        "Pacifists",
        "Militaristic",
        "Spiritual",
        "Scientific",
        "Chaotic"
    };
    
    /// <summary>
    /// Get interaction rules for a disposition (to be implemented later)
    /// </summary>
    public static string GetInteractionRules(string disposition)
    {
        return disposition switch
        {
            "Peaceful" => "Generally friendly, prefer diplomatic solutions",
            "Aggressive" => "Quick to attack, difficult to negotiate with",
            "Traders" => "Always interested in commerce, neutral in conflicts",
            "Warriors" => "Respect strength, honor combat prowess",
            "Scholars" => "Value knowledge exchange, interested in technology",
            "Explorers" => "Curious about new territories, willing to share maps",
            "Isolationists" => "Prefer to be left alone, defensive when approached",
            "Expansionists" => "Constantly seeking new territory, competitive",
            "Mercenaries" => "Motivated by profit, services for hire",
            "Pirates" => "Hostile to trade vessels, may demand tribute",
            "Diplomats" => "Skilled negotiators, seek peaceful resolutions",
            "Xenophobic" => "Fear/hate other species, difficult relations",
            "Xenophilic" => "Love meeting new species, very friendly",
            "Nomadic" => "Constantly moving, temporary alliances",
            "Territorial" => "Defend their space aggressively",
            "Pacifists" => "Refuse to engage in violence",
            "Militaristic" => "Organized, disciplined, respect hierarchy",
            "Spiritual" => "Guided by religious beliefs, may proselytize",
            "Scientific" => "Rational, interested in research cooperation",
            "Chaotic" => "Unpredictable behavior, random reactions",
            _ => "Unknown disposition behavior"
        };
    }
}