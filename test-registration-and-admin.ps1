# Test Registration and Admin Panel for ChronoVoid 2500

Write-Host "🚀 ChronoVoid 2500 - Registration & Admin Test" -ForegroundColor Green
Write-Host "=============================================" -ForegroundColor Green

# Test registration directly via API first
Write-Host "`n🧪 Testing API Registration..." -ForegroundColor Yellow
try {
    $testUser = @{
        username = "apitest_$(Get-Random -Maximum 9999)"
        email = "apitest$(Get-Random -Maximum 9999)@example.com"
        password = "password123"
    } | ConvertTo-Json

    $result = Invoke-RestMethod -Uri "https://localhost:7001/api/auth/register" -Method Post -Body $testUser -ContentType "application/json" -SkipCertificateCheck
    Write-Host "✅ API Registration successful!" -ForegroundColor Green
    Write-Host "   User ID: $($result.userId)" -ForegroundColor Cyan
    Write-Host "   Username: $($result.username)" -ForegroundColor Cyan
    Write-Host "   Cargo Holds: $($result.cargoHolds)" -ForegroundColor Cyan
}
catch {
    Write-Host "❌ API Registration failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Check if API is running
Write-Host "`n🔍 Checking API status..." -ForegroundColor Yellow
try {
    $apiStats = Invoke-RestMethod -Uri "https://localhost:7001/api/admin/stats" -Method Get -SkipCertificateCheck
    Write-Host "✅ API is running on https://localhost:7001" -ForegroundColor Green
    Write-Host "   📊 Current Stats:" -ForegroundColor Cyan
    Write-Host "      • Total Users: $($apiStats.totalUsers)" -ForegroundColor White
    Write-Host "      • Total Realms: $($apiStats.totalRealms)" -ForegroundColor White
    Write-Host "      • Total Nodes: $($apiStats.totalNodes)" -ForegroundColor White
}
catch {
    Write-Host "❌ API is not running. Please start it first:" -ForegroundColor Red
    Write-Host "   cd ChronoVoid.API && dotnet run --launch-profile https" -ForegroundColor Yellow
    exit 1
}

# Check existing users
Write-Host "`n👥 Checking existing users..." -ForegroundColor Yellow
try {
    $existingUsers = Invoke-RestMethod -Uri "https://localhost:7001/api/auth/debug/users" -Method Get -SkipCertificateCheck
    if ($existingUsers.count -gt 0) {
        Write-Host "⚠️  Found $($existingUsers.count) existing users:" -ForegroundColor Yellow
        foreach ($user in $existingUsers.users) {
            Write-Host "   • $($user.username) ($($user.email))" -ForegroundColor White
        }
        
        $clearUsers = Read-Host "`nDo you want to clear all existing users? (y/N)"
        if ($clearUsers -eq 'y' -or $clearUsers -eq 'Y') {
            Write-Host "🗑️  Clearing all users..." -ForegroundColor Yellow
            try {
                $result = Invoke-RestMethod -Uri "https://localhost:7001/api/admin/clear-all-users" -Method Delete -SkipCertificateCheck
                Write-Host "✅ $($result.message)" -ForegroundColor Green
            }
            catch {
                Write-Host "❌ Error clearing users: $($_.Exception.Message)" -ForegroundColor Red
            }
        }
    } else {
        Write-Host "✅ No existing users found - ready for testing" -ForegroundColor Green
    }
}
catch {
    Write-Host "❌ Error checking users: $($_.Exception.Message)" -ForegroundColor Red
}

# Open admin panel
Write-Host "`n🌐 Opening Admin Panel..." -ForegroundColor Yellow
Write-Host "   URL: https://localhost:7001/admin.html" -ForegroundColor Cyan
try {
    Start-Process "https://localhost:7001/admin.html"
    Write-Host "✅ Admin panel opened in browser" -ForegroundColor Green
}
catch {
    Write-Host "⚠️  Could not auto-open browser. Please manually navigate to:" -ForegroundColor Yellow
    Write-Host "   https://localhost:7001/admin.html" -ForegroundColor Cyan
}

# Start mobile app
Write-Host "`n📱 Starting Mobile App..." -ForegroundColor Yellow
Write-Host "   The mobile app will start with the debug page" -ForegroundColor Cyan
Write-Host "   Navigate to Login and try creating a new account" -ForegroundColor Cyan

$startMobile = Read-Host "`nStart mobile app now? (Y/n)"
if ($startMobile -ne 'n' -and $startMobile -ne 'N') {
    try {
        Set-Location "ChronoVoid2500.Mobile"
        Write-Host "🚀 Launching mobile app..." -ForegroundColor Green
        
        # Start mobile app in background
        $mobileProcess = Start-Process -FilePath "dotnet" -ArgumentList "run", "-f", "net10.0-windows10.0.19041.0" -PassThru -WindowStyle Normal
        
        Write-Host "✅ Mobile app started (Process ID: $($mobileProcess.Id))" -ForegroundColor Green
        Write-Host "   Use the debug page to navigate to Login" -ForegroundColor Cyan
    }
    catch {
        Write-Host "❌ Error starting mobile app: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "   Try running manually: cd ChronoVoid2500.Mobile && dotnet run -f net10.0-windows10.0.19041.0" -ForegroundColor Yellow
    }
}

Write-Host "`n📋 Test Instructions:" -ForegroundColor Magenta
Write-Host "=====================" -ForegroundColor Magenta
Write-Host "1. 🌐 Admin Panel (already opened):" -ForegroundColor White
Write-Host "   • View real-time stats and user management" -ForegroundColor Gray
Write-Host "   • Click on users to see detailed information" -ForegroundColor Gray
Write-Host "   • Monitor realm activity and user locations" -ForegroundColor Gray

Write-Host "`n2. 📱 Mobile App:" -ForegroundColor White
Write-Host "   • Start with debug page showing Shell.Current status" -ForegroundColor Gray
Write-Host "   • Navigate to Login page using debug controls" -ForegroundColor Gray
Write-Host "   • Try creating a new account (should work now)" -ForegroundColor Gray
Write-Host "   • Test the full flow: Register → Realms → Game" -ForegroundColor Gray

Write-Host "`n3. 🔄 Real-time Testing:" -ForegroundColor White
Write-Host "   • Create users in mobile app" -ForegroundColor Gray
Write-Host "   • Refresh admin panel to see new users appear" -ForegroundColor Gray
Write-Host "   • Click on users to see their realm activity" -ForegroundColor Gray
Write-Host "   • Test user management features (reset, delete)" -ForegroundColor Gray

Write-Host "`n🎯 Expected Results:" -ForegroundColor Cyan
Write-Host "• Registration should work without 'user exists' errors" -ForegroundColor Green
Write-Host "• Admin panel should show users and their realm activity" -ForegroundColor Green
Write-Host "• User details should display current location and history" -ForegroundColor Green
Write-Host "• Real-time stats should update as users join realms" -ForegroundColor Green

Write-Host "`n🛠️  Troubleshooting:" -ForegroundColor Yellow
Write-Host "• If registration fails: Check API logs and database connection" -ForegroundColor Gray
Write-Host "• If admin panel doesn't load: Verify API is running on port 7001" -ForegroundColor Gray
Write-Host "• If mobile app crashes: Check debug page for Shell.Current status" -ForegroundColor Gray

Write-Host "`n✨ Ready for testing! Press any key when done..." -ForegroundColor Green
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

# Cleanup
Write-Host "`n🧹 Cleanup..." -ForegroundColor Yellow
if ($mobileProcess -and !$mobileProcess.HasExited) {
    Write-Host "Stopping mobile app..." -ForegroundColor Gray
    $mobileProcess.Kill()
}

Write-Host "✅ Test completed!" -ForegroundColor Green