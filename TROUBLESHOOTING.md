# ChronoVoid 2500 - Troubleshooting Guide

## Shell.Current Null Exception Issue

### Problem
The mobile app crashes immediately on startup with:
```
Microsoft.Maui.Controls.Shell.Current.get returned null
```

### Root Cause
This happens when code tries to access `Shell.Current` before the Shell has been fully initialized by the MAUI framework.

### Solutions Implemented

#### 1. Debug Page
- Added a dedicated debug page (`Views/DebugPage.xaml`) that starts first
- Shows real-time Shell.Current status
- Provides test buttons for navigation and alerts
- Logs all Shell lifecycle events

#### 2. Safe Navigation Helper
- Created `Debug/DebugHelper.cs` with safe navigation methods
- All ViewModels now use `DebugHelper.SafeNavigateAsync()` instead of direct Shell.Current calls
- Gracefully handles null Shell.Current scenarios

#### 3. Enhanced Logging
- Added comprehensive debug logging throughout the app lifecycle
- App.xaml.cs now logs Shell creation and lifecycle events
- All navigation attempts are logged with timestamps

### How to Debug

#### Step 1: Run the Debug Version
```bash
.\run-debug-mobile.ps1
```

#### Step 2: Check Debug Page
The app now starts with a debug page that shows:
- ✅/❌ Shell.Current availability status
- Shell type and current page information
- Test buttons for navigation and alerts
- Real-time debug log

#### Step 3: Test Navigation
Use the debug page buttons to test:
- "Test Navigate to Login" - Tests Shell navigation
- "Test Alert Dialog" - Tests Shell alert dialogs
- "Check Shell.Current" - Refreshes Shell status

#### Step 4: Review Debug Output
Check the debug log on the page and in Visual Studio Output window for:
- Shell lifecycle events
- Navigation attempts and results
- Error messages with timestamps

### Common Issues and Fixes

#### Issue: Shell.Current is Always Null
**Cause**: Shell not properly initialized
**Fix**: 
1. Check App.xaml.cs CreateWindow method
2. Ensure AppShell is created correctly
3. Verify Shell events are firing (check debug log)

#### Issue: Navigation Fails Silently
**Cause**: Invalid route or Shell not ready
**Fix**:
1. Verify route names in AppShell.xaml
2. Check if Shell.Current is available when navigating
3. Use DebugHelper.SafeNavigateAsync() instead of direct calls

#### Issue: App Crashes on Startup
**Cause**: Exception in ViewModels or dependency injection
**Fix**:
1. Check MauiProgram.cs service registrations
2. Verify all dependencies are properly registered
3. Check constructor parameters in ViewModels

### Testing Strategy

#### Unit Testing Limitations
- MAUI projects can't be directly referenced by standard test projects
- Shell.Current requires MAUI runtime environment
- UI testing requires Appium setup (complex)

#### Alternative Testing Approaches
1. **Debug Page Testing**: Use the built-in debug page for manual testing
2. **ViewModel Testing**: Test ViewModels in isolation without Shell dependencies
3. **API Testing**: Test the backend API separately
4. **Integration Testing**: Test full app flow manually with debug logging

### Debug Page Features

#### Shell Status Monitor
- Real-time Shell.Current availability
- Shell type and page information
- Automatic status refresh

#### Navigation Testing
- Safe navigation to different pages
- Error handling and logging
- Route validation

#### Alert Testing
- Test Shell alert dialogs
- Error handling for null Shell
- Response logging

#### Debug Log
- Timestamped debug messages
- Shell lifecycle events
- Navigation attempts and results
- Error messages and stack traces

### Performance Considerations

#### Debug Mode Only
- Debug helpers are designed for development only
- Remove or disable debug logging in production
- Debug page should not be accessible in release builds

#### Memory Usage
- Debug logging accumulates in memory
- Clear debug log regularly during testing
- Monitor memory usage during extended testing

### Next Steps

#### If Shell.Current Works
1. Navigate to login page using debug page
2. Test full app flow: Login → Realms → Game
3. Remove debug page from production build

#### If Shell.Current Still Null
1. Check MAUI framework version compatibility
2. Verify Windows development environment setup
3. Try creating minimal MAUI app to isolate issue
4. Check for conflicting packages or dependencies

### Environment Requirements

#### Development Environment
- .NET 10.0 SDK (preview)
- Visual Studio 2022 with MAUI workload
- Windows 10/11 with developer mode enabled

#### Runtime Requirements
- Windows 10 version 19041 or higher
- .NET 10.0 runtime
- WebView2 runtime (for MAUI apps)

### Additional Resources

#### MAUI Documentation
- [Shell Navigation](https://learn.microsoft.com/dotnet/maui/fundamentals/shell/navigation)
- [MAUI Troubleshooting](https://learn.microsoft.com/dotnet/maui/troubleshooting)

#### Debug Tools
- Visual Studio Debugger
- MAUI Hot Reload
- Device logs and crash reports

#### Community Support
- [MAUI GitHub Issues](https://github.com/dotnet/maui/issues)
- [Microsoft Q&A](https://docs.microsoft.com/answers/topics/dotnet-maui.html)