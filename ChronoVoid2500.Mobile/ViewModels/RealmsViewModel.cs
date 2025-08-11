using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ChronoVoid2500.Mobile.Models;
using ChronoVoid2500.Mobile.Services;
using ChronoVoid2500.Mobile.Debug;

namespace ChronoVoid2500.Mobile.ViewModels;

[QueryProperty(nameof(UserData), "UserData")]
public partial class RealmsViewModel : BaseViewModel
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private AuthResponse? userData;

    [ObservableProperty]
    private ObservableCollection<RealmDto> realms = new();

    [ObservableProperty]
    private RealmDto? selectedRealm;

    public RealmsViewModel(ApiService apiService)
    {
        _apiService = apiService;
        Title = "Select Realm";
    }

    public async Task InitializeAsync()
    {
        await LoadRealmsAsync();
    }

    [RelayCommand]
    private async Task LoadRealmsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ClearError();

            var realmsData = await _apiService.GetRealmsAsync();
            if (realmsData != null)
            {
                Realms.Clear();
                foreach (var realm in realmsData)
                {
                    // Validate realm data before adding
                    if (realm != null)
                    {
                        // Log realm data for debugging
                        System.Diagnostics.Debug.WriteLine($"Loading realm: ID={realm.Id}, Name={realm.Name ?? "NULL"}, NodeCount={realm.NodeCount}");
                        Realms.Add(realm);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Warning: Null realm data received from API");
                    }
                }
                
                System.Diagnostics.Debug.WriteLine($"Total realms loaded: {Realms.Count}");
            }
            else
            {
                SetError("Failed to load realms");
                System.Diagnostics.Debug.WriteLine("GetRealmsAsync returned null");
            }
        }
        catch (Exception ex)
        {
            SetError($"Error loading realms: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"LoadRealmsAsync error: {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SelectRealmAsync(RealmDto realm)
    {
        if (IsBusy || UserData == null || realm == null) return;

        try
        {
            SelectedRealm = realm;

            // Validate realm data before showing dialog
            var realmName = realm.Name ?? "Unknown Realm";
            var nodeCount = realm.NodeCount;
            var quantumRate = realm.QuantumStationSeedRate;

            // Show confirmation dialog
            bool answer = await DebugHelper.SafeDisplayAlertAsync(
                "Join Realm", 
                $"Do you want to join '{realmName}'?\n\nNodes: {nodeCount}\nQuantum Stations: {quantumRate}%", 
                "Join", 
                "Cancel");

            if (answer)
            {
                await JoinRealmAsync(realm);
            }
        }
        catch (Exception ex)
        {
            SetError($"Error selecting realm: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"SelectRealmAsync error: {ex}");
        }
    }

    private async Task JoinRealmAsync(RealmDto realm)
    {
        try
        {
            IsBusy = true;
            ClearError();

            // Additional validation
            if (UserData == null)
            {
                SetError("User data is not available");
                return;
            }

            if (realm == null)
            {
                SetError("Realm data is not available");
                return;
            }

            var request = new JoinRealmRequest
            {
                UserId = UserData.UserId,
                RealmId = realm.Id
            };

            var response = await _apiService.JoinRealmAsync(request);
            if (response != null && response.Success)
            {
                // Validate response data before navigation
                if (response.StartingNodeId == null || response.StartingNode == null)
                {
                    SetError("Invalid response from server - missing starting node information");
                    return;
                }

                // Update user data with new realm info
                UserData.RealmId = realm.Id;
                UserData.CurrentNodeId = response.StartingNodeId;

                // Navigate to game screen
                await DebugHelper.SafeNavigateAsync("//game", new Dictionary<string, object>
                {
                    ["UserData"] = UserData,
                    ["CurrentNode"] = response.StartingNode
                });
            }
            else
            {
                SetError(response?.Message ?? "Failed to join realm");
            }
        }
        catch (Exception ex)
        {
            SetError($"Error joining realm: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"JoinRealmAsync error: {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await DebugHelper.SafeNavigateAsync("//login");
    }
}