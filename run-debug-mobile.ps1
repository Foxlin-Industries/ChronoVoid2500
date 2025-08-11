# Run ChronoVoid 2500 Mobile App in Debug Mode

Write-Host "🚀 Starting ChronoVoid 2500 Mobile App (Debug Mode)..." -ForegroundColor Green

# Check if API is running
try {
    $response = Invoke-WebRequest -Uri "https://localhost:7001/api/realm" -Method Get -TimeoutSec 5 -SkipCertificateCheck
    Write-Host "✅ API is running and accessible" -ForegroundColor Green
}
catch {
    Write-Host "⚠️  API is not running at https://localhost:7001" -ForegroundColor Yellow
    Write-Host "   The mobile app will still start but won't be able to connect to the backend." -ForegroundColor Yellow
    Write-Host "   To start the API: cd ChronoVoid.API && dotnet run" -ForegroundColor Cyan
}

Write-Host "`n🔧 Starting mobile app with debug page..." -ForegroundColor Yellow
Write-Host "   The app will start with a debug page that shows Shell.Current status" -ForegroundColor White
Write-Host "   Use the debug page to test navigation and identify any issues" -ForegroundColor White

Set-Location "ChronoVoid2500.Mobile"

# Run the mobile app
Write-Host "`n▶️  Launching mobile app..." -ForegroundColor Cyan
dotnet run -f net10.0-windows10.0.19041.0

Write-Host "`n📝 Debug Tips:" -ForegroundColor Magenta
Write-Host "• Check the debug page for Shell.Current status" -ForegroundColor White
Write-Host "• Test navigation buttons to see if Shell is working" -ForegroundColor White
Write-Host "• Look at the debug log for detailed information" -ForegroundColor White
Write-Host "• If Shell.Current is null, the issue is in app initialization" -ForegroundColor White