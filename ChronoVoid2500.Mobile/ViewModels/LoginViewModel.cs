using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ChronoVoid2500.Mobile.Models;
using ChronoVoid2500.Mobile.Services;
using ChronoVoid2500.Mobile.Debug;

namespace ChronoVoid2500.Mobile.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private bool isRegistering = false;

    [ObservableProperty]
    private bool isPasswordHidden = true;

    public LoginViewModel(ApiService apiService)
    {
        _apiService = apiService;
        Title = "ChronoVoid 2500";
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            SetError("Please enter username and password");
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            var request = new LoginRequest
            {
                Username = Username,
                Password = Password
            };

            var response = await _apiService.LoginAsync(request);
            if (response != null)
            {
                // Store user data and navigate to realm selection
                await DebugHelper.SafeNavigateAsync("//realms", new Dictionary<string, object>
                {
                    ["UserData"] = response
                });
            }
            else
            {
                SetError("Invalid username or password");
            }
        }
        catch (Exception ex)
        {
            SetError($"Login failed: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Email))
        {
            SetError("Please fill in all fields");
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            var request = new RegisterRequest
            {
                Username = Username,
                Email = Email,
                Password = Password
            };

            var response = await _apiService.RegisterAsync(request);
            if (response != null)
            {
                // Store user data and navigate to realm selection
                await DebugHelper.SafeNavigateAsync("//realms", new Dictionary<string, object>
                {
                    ["UserData"] = response
                });
            }
            else
            {
                SetError("Registration failed. Username or email may already exist.");
            }
        }
        catch (Exception ex)
        {
            SetError($"Registration failed: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ToggleRegistration()
    {
        IsRegistering = !IsRegistering;
        ClearError();
        Title = IsRegistering ? "Create Account" : "ChronoVoid 2500";
    }

    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordHidden = !IsPasswordHidden;
    }

    [RelayCommand]
    private void FocusNextField(string fieldName)
    {
        // This will be handled in the code-behind for field focus management
    }
}