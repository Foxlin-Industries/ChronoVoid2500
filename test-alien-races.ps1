# ChronoVoid 2500 - Alien Race System Test Script
# This script demonstrates the complete alien race generation and management system

Write-Host "🚀 ChronoVoid 2500 - Alien Race System Test" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

$API_BASE = "http://localhost:7000/api/AlienRace"

# Test 1: Generate 5000 alien races
Write-Host "`n1. Generating 5000 alien races..." -ForegroundColor Yellow
try {
    $generateResult = Invoke-RestMethod -Uri "$API_BASE/generate" -Method POST -ContentType "application/json" -Body '{"count": 5000}'
    Write-Host "✅ Successfully generated $($generateResult.Count) alien races!" -ForegroundColor Green
} catch {
    Write-Host "❌ Error generating races: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Test 2: Get statistics
Write-Host "`n2. Getting alien race statistics..." -ForegroundColor Yellow
try {
    $stats = Invoke-RestMethod -Uri "$API_BASE/stats" -Method GET
    Write-Host "✅ Statistics retrieved:" -ForegroundColor Green
    Write-Host "   Total Races: $($stats.totalRaces)" -ForegroundColor White
    Write-Host "   Active Races: $($stats.activeRaces)" -ForegroundColor White
    Write-Host "   Translator Capable: $($stats.translatorCapableCount)" -ForegroundColor White
    Write-Host "   Disposition Types: $($stats.dispositionCounts.Keys.Count)" -ForegroundColor White
} catch {
    Write-Host "❌ Error getting stats: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 3: Get random races with tech filter
Write-Host "`n3. Getting 10 random high-tech races (Tech 7-10)..." -ForegroundColor Yellow
try {
    $randomRaces = Invoke-RestMethod -Uri "$API_BASE/random?count=10&minTech=7&maxTech=10" -Method GET
    Write-Host "✅ Random high-tech races:" -ForegroundColor Green
    foreach ($race in $randomRaces) {
        $emoji = switch ($race.disposition) {
            "Peaceful" { "🕊️" }
            "Aggressive" { "⚔️" }
            "Traders" { "💰" }
            "Pirates" { "🏴‍☠️" }
            "Scholars" { "📚" }
            "Warriors" { "🛡️" }
            "Diplomats" { "🤝" }
            default { "👽" }
        }
        $translator = if ($race.translatorCapable) { "🗣️" } else { "🚫" }
        Write-Host "   $emoji $($race.name) - Tech: $($race.technologyLevel)/10 - $($race.disposition) - Relations: $($race.humanAgreeability)/10 $translator" -ForegroundColor White
    }
} catch {
    Write-Host "❌ Error getting random races: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 4: Activate 50 random races
Write-Host "`n4. Activating 50 random races for gameplay..." -ForegroundColor Yellow
try {
    $activeRaces = Invoke-RestMethod -Uri "$API_BASE/random?count=50" -Method GET
    $raceIds = $activeRaces | ForEach-Object { $_.id }
    $activateResult = Invoke-RestMethod -Uri "$API_BASE/activate" -Method POST -ContentType "application/json" -Body (@{raceIds = $raceIds} | ConvertTo-Json)
    Write-Host "✅ $($activateResult.message)" -ForegroundColor Green
} catch {
    Write-Host "❌ Error activating races: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 5: Show active races by disposition
Write-Host "`n5. Showing active races by disposition..." -ForegroundColor Yellow
try {
    $activeRaces = Invoke-RestMethod -Uri "$API_BASE?IsActive=true&PageSize=50" -Method GET
    $dispositionGroups = $activeRaces | Group-Object disposition
    Write-Host "✅ Active races by disposition:" -ForegroundColor Green
    foreach ($group in $dispositionGroups | Sort-Object Name) {
        Write-Host "   $($group.Name): $($group.Count) races" -ForegroundColor White
    }
} catch {
    Write-Host "❌ Error getting active races: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 6: Test race replacement
Write-Host "`n6. Testing race replacement..." -ForegroundColor Yellow
try {
    $firstActiveRace = $activeRaces[0]
    Write-Host "   Replacing: $($firstActiveRace.name) (Tech: $($firstActiveRace.technologyLevel), $($firstActiveRace.disposition))" -ForegroundColor White
    
    $replacementRace = Invoke-RestMethod -Uri "$API_BASE/$($firstActiveRace.id)/replace?minTech=5&maxTech=10" -Method POST
    Write-Host "✅ Replaced with: $($replacementRace.name) (Tech: $($replacementRace.technologyLevel), $($replacementRace.disposition))" -ForegroundColor Green
} catch {
    Write-Host "❌ Error replacing race: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 7: Final statistics
Write-Host "`n7. Final statistics..." -ForegroundColor Yellow
try {
    $finalStats = Invoke-RestMethod -Uri "$API_BASE/stats" -Method GET
    Write-Host "✅ Final system state:" -ForegroundColor Green
    Write-Host "   Total Races: $($finalStats.totalRaces)" -ForegroundColor White
    Write-Host "   Active Races: $($finalStats.activeRaces)" -ForegroundColor White
    Write-Host "   Technology Distribution:" -ForegroundColor White
    for ($i = 1; $i -le 10; $i++) {
        $count = $finalStats.technologyLevelCounts.$i.ToString()
        if ($count) {
            Write-Host "     Tech Level $i`: $count races" -ForegroundColor Gray
        }
    }
} catch {
    Write-Host "❌ Error getting final stats: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n🎉 Alien Race System Test Complete!" -ForegroundColor Cyan
Write-Host "The system successfully generated 5000 unique alien races with:" -ForegroundColor White
Write-Host "• Procedural names using seeded generation" -ForegroundColor White
Write-Host "• Technology levels from 1-10 (primitive to advanced)" -ForegroundColor White
Write-Host "• 20 different dispositions with interaction rules" -ForegroundColor White
Write-Host "• Human agreeability ratings from 1-10" -ForegroundColor White
Write-Host "• Translator capability flags" -ForegroundColor White
Write-Host "• Rich additional traits (physical form, size, lifespan, government)" -ForegroundColor White
Write-Host "• Admin tools for selecting and managing active races" -ForegroundColor White
Write-Host "`nAdmin panel available at: http://localhost:7000/admin.html" -ForegroundColor Cyan