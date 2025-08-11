# ChronoVoid 2500 Test Setup Script

Write-Host "🚀 ChronoVoid 2500 - Setting up test environment..." -ForegroundColor Green

# Test API build
Write-Host "`n📡 Testing API build..." -ForegroundColor Yellow
Set-Location "ChronoVoid.API"
$apiBuild = dotnet build --verbosity quiet
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ API build successful" -ForegroundColor Green
} else {
    Write-Host "❌ API build failed" -ForegroundColor Red
    exit 1
}

# Test Mobile build
Write-Host "`n📱 Testing Mobile app build..." -ForegroundColor Yellow
Set-Location "..\ChronoVoid2500.Mobile"
$mobileBuild = dotnet build -f net10.0-windows10.0.19041.0 --verbosity quiet
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Mobile app build successful" -ForegroundColor Green
} else {
    Write-Host "❌ Mobile app build failed" -ForegroundColor Red
    exit 1
}

Set-Location ".."

Write-Host "`n🎮 Setup complete! Ready to run ChronoVoid 2500" -ForegroundColor Green
Write-Host "`nNext steps:" -ForegroundColor Cyan
Write-Host "1. Set up PostgreSQL database" -ForegroundColor White
Write-Host "2. Update connection string in ChronoVoid.API/appsettings.json" -ForegroundColor White
Write-Host "3. Run: cd ChronoVoid.API && dotnet ef database update" -ForegroundColor White
Write-Host "4. Start API: cd ChronoVoid.API && dotnet run" -ForegroundColor White
Write-Host "5. Start Mobile: cd ChronoVoid2500.Mobile && dotnet run -f net10.0-windows10.0.19041.0" -ForegroundColor White

Write-Host "`n🌟 Features implemented:" -ForegroundColor Magenta
Write-Host "• User authentication (register/login)" -ForegroundColor White
Write-Host "• Realm selection and joining" -ForegroundColor White
Write-Host "• Node navigation with hyper-tunnels" -ForegroundColor White
Write-Host "• Star names and planet counts" -ForegroundColor White
Write-Host "• Quantum station indicators" -ForegroundColor White
Write-Host "• Ship creation with 300 cargo holds" -ForegroundColor White
Write-Host "• Real-time responsive UI" -ForegroundColor White