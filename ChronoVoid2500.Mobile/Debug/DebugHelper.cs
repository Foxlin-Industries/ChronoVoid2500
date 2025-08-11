using System.Diagnostics;

namespace ChronoVoid2500.Mobile.Debug;

public static class DebugHelper
{
    public static void LogShellState(string location)
    {
        try
        {
            var shellCurrent = Shell.Current;
            var message = $"[{location}] Shell.Current: {(shellCurrent != null ? "Available" : "NULL")}";
            
            System.Diagnostics.Debug.WriteLine(message);
            Console.WriteLine(message);
            
            if (shellCurrent != null)
            {
                System.Diagnostics.Debug.WriteLine($"[{location}] Shell Type: {shellCurrent.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"[{location}] Current Page: {shellCurrent.CurrentPage?.GetType().Name ?? "NULL"}");
            }
        }
        catch (Exception ex)
        {
            var errorMessage = $"[{location}] Error checking Shell.Current: {ex.Message}";
            System.Diagnostics.Debug.WriteLine(errorMessage);
            Console.WriteLine(errorMessage);
        }
    }

    public static async Task SafeNavigateAsync(string route, Dictionary<string, object>? parameters = null)
    {
        try
        {
            LogShellState("SafeNavigateAsync");
            
            if (Shell.Current == null)
            {
                System.Diagnostics.Debug.WriteLine($"Cannot navigate to {route} - Shell.Current is null");
                return;
            }

            if (parameters != null)
            {
                await Shell.Current.GoToAsync(route, parameters);
            }
            else
            {
                await Shell.Current.GoToAsync(route);
            }
            
            System.Diagnostics.Debug.WriteLine($"Successfully navigated to {route}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Navigation error to {route}: {ex.Message}");
            Console.WriteLine($"Navigation error to {route}: {ex.Message}");
        }
    }

    public static async Task<bool> SafeDisplayAlertAsync(string title, string message, string accept, string? cancel = null)
    {
        try
        {
            LogShellState("SafeDisplayAlertAsync");
            
            if (Shell.Current == null)
            {
                System.Diagnostics.Debug.WriteLine($"Cannot show alert '{title}' - Shell.Current is null");
                return false;
            }

            if (cancel != null)
            {
                return await Shell.Current.DisplayAlertAsync(title, message, accept, cancel);
            }
            else
            {
                await Shell.Current.DisplayAlertAsync(title, message, accept);
                return true;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Alert error: {ex.Message}");
            Console.WriteLine($"Alert error: {ex.Message}");
            return false;
        }
    }
}