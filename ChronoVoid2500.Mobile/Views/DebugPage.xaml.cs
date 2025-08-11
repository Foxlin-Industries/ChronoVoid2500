using ChronoVoid2500.Mobile.Debug;
using System.Text;

namespace ChronoVoid2500.Mobile.Views;

public partial class DebugPage : ContentPage
{
    private readonly StringBuilder _debugLog = new();

    public DebugPage()
    {
        InitializeComponent();
        LogMessage("DebugPage constructor called");
        CheckShellStatus();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LogMessage("DebugPage OnAppearing called");
        CheckShellStatus();
    }

    private void CheckShellStatus()
    {
        try
        {
            var shellCurrent = Shell.Current;
            var status = shellCurrent != null ? "✅ Available" : "❌ NULL";
            
            ShellStatusLabel.Text = $"Shell.Current: {status}";
            
            if (shellCurrent != null)
            {
                ShellStatusLabel.Text += $"\nShell Type: {shellCurrent.GetType().Name}";
                ShellStatusLabel.Text += $"\nCurrent Page: {shellCurrent.CurrentPage?.GetType().Name ?? "NULL"}";
            }
            
            LogMessage($"Shell.Current status: {status}");
        }
        catch (Exception ex)
        {
            ShellStatusLabel.Text = $"❌ Error: {ex.Message}";
            LogMessage($"Error checking Shell.Current: {ex.Message}");
        }
    }

    private void OnCheckShellClicked(object sender, EventArgs e)
    {
        LogMessage("Check Shell button clicked");
        CheckShellStatus();
    }

    private async void OnNavigateToLoginClicked(object sender, EventArgs e)
    {
        LogMessage("Navigate to Login button clicked");
        
        try
        {
            if (Shell.Current == null)
            {
                LogMessage("❌ Cannot navigate - Shell.Current is null");
                return;
            }

            await Shell.Current.GoToAsync("//login");
            LogMessage("✅ Navigation to login successful");
        }
        catch (Exception ex)
        {
            LogMessage($"❌ Navigation error: {ex.Message}");
        }
    }

    private async void OnTestAlertClicked(object sender, EventArgs e)
    {
        LogMessage("Test Alert button clicked");
        
        try
        {
            if (Shell.Current == null)
            {
                LogMessage("❌ Cannot show alert - Shell.Current is null");
                return;
            }

            await Shell.Current.DisplayAlertAsync("Test", "This is a test alert", "OK");
            LogMessage("✅ Alert displayed successfully");
        }
        catch (Exception ex)
        {
            LogMessage($"❌ Alert error: {ex.Message}");
        }
    }

    private void OnClearLogClicked(object sender, EventArgs e)
    {
        _debugLog.Clear();
        DebugLogLabel.Text = "Debug log cleared...";
    }

    private void LogMessage(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        var logEntry = $"[{timestamp}] {message}";
        
        _debugLog.AppendLine(logEntry);
        DebugLogLabel.Text = _debugLog.ToString();
        
        System.Diagnostics.Debug.WriteLine(logEntry);
        Console.WriteLine(logEntry);
    }
}