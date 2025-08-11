# Create Test Realm Script for ChronoVoid 2500

param(
    [string]$ApiUrl = "https://localhost:7001",
    [string]$RealmName = "Test Galaxy Alpha",
    [int]$NodeCount = 25,
    [int]$QuantumStationRate = 30,
    [bool]$NoDeadNodes = $true
)

Write-Host "🌌 Creating test realm: $RealmName" -ForegroundColor Green

$body = @{
    Name = $RealmName
    NodeCount = $NodeCount
    QuantumStationSeedRate = $QuantumStationRate
    NoDeadNodes = $NoDeadNodes
} | ConvertTo-Json

try {
    # Skip SSL certificate validation for localhost testing
    [System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}
    
    $response = Invoke-RestMethod -Uri "$ApiUrl/api/realm" -Method Post -Body $body -ContentType "application/json"
    
    Write-Host "✅ Realm created successfully!" -ForegroundColor Green
    Write-Host "Realm ID: $($response.Id)" -ForegroundColor Cyan
    Write-Host "Name: $($response.Name)" -ForegroundColor Cyan
    Write-Host "Nodes: $($response.NodeCount)" -ForegroundColor Cyan
    Write-Host "Quantum Stations: $($response.QuantumStationSeedRate)%" -ForegroundColor Cyan
    Write-Host "No Dead Nodes: $($response.NoDeadNodes)" -ForegroundColor Cyan
    
    Write-Host "`n🎮 Ready to test! Start the mobile app and join this realm." -ForegroundColor Yellow
}
catch {
    Write-Host "❌ Failed to create realm: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Make sure the API is running at $ApiUrl" -ForegroundColor Yellow
}

# Usage examples:
Write-Host "`n📝 Usage examples:" -ForegroundColor Magenta
Write-Host ".\create-test-realm.ps1" -ForegroundColor White
Write-Host ".\create-test-realm.ps1 -RealmName 'Beta Sector' -NodeCount 50 -QuantumStationRate 20" -ForegroundColor White