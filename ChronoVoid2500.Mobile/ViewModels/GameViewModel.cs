using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ChronoVoid2500.Mobile.Models;
using ChronoVoid2500.Mobile.Services;
using ChronoVoid2500.Mobile.Debug;

namespace ChronoVoid2500.Mobile.ViewModels;

[QueryProperty(nameof(UserData), "UserData")]
[QueryProperty(nameof(CurrentNode), "CurrentNode")]
public partial class GameViewModel : BaseViewModel
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private AuthResponse? userData;

    [ObservableProperty]
    private NodeDetailDto? currentNode;

    [ObservableProperty]
    private ObservableCollection<ConnectedNodeDto> availableNodes = new();

    [ObservableProperty]
    private bool showNavigation = false;

    [ObservableProperty]
    private int cargoHolds;

    [ObservableProperty]
    private string playerInfo = string.Empty;

    [ObservableProperty]
    private string? selectedPlanetName;

    // Dashboard display properties
    public string LocationDisplay
    {
        get
        {
            if (CurrentNode == null) return "📍 System Unknown";
            
            var systemName = string.IsNullOrEmpty(CurrentNode.StarName) ? $"System {CurrentNode.NodeNumber}" : CurrentNode.StarName;
            
            if (!string.IsNullOrEmpty(SelectedPlanetName))
                return $"📍 {systemName} - {SelectedPlanetName}";
            
            return $"📍 {systemName}";
        }
    }

    public string PlanetCountDisplay
    {
        get
        {
            if (CurrentNode == null) return "🪐 Planets: 0";
            return $"🪐 Planets: {CurrentNode.PlanetCount}";
        }
    }

    public bool HasPlanets => CurrentNode?.PlanetCount > 0;
    public bool HasQuantumStation => CurrentNode?.HasQuantumStation == true;

    public GameViewModel(ApiService apiService)
    {
        _apiService = apiService;
        Title = "ChronoVoid 2500";
    }



    partial void OnCurrentNodeChanged(NodeDetailDto? value)
    {
        if (value != null)
        {
            Title = $"System {value.NodeNumber} - {value.StarName}";
            AvailableNodes.Clear();
            foreach (var node in value.ConnectedNodes)
            {
                AvailableNodes.Add(node);
            }
            UpdatePlayerInfo();
        }
        
        // Clear selected planet when changing systems
        SelectedPlanetName = null;
        
        // Notify dashboard properties have changed
        OnPropertyChanged(nameof(LocationDisplay));
        OnPropertyChanged(nameof(PlanetCountDisplay));
        OnPropertyChanged(nameof(HasPlanets));
        OnPropertyChanged(nameof(HasQuantumStation));
    }

    partial void OnSelectedPlanetNameChanged(string? value)
    {
        OnPropertyChanged(nameof(LocationDisplay));
    }

    partial void OnUserDataChanged(AuthResponse? value)
    {
        if (value != null)
        {
            CargoHolds = value.CargoHolds;
            UpdatePlayerInfo();
        }
    }

    private void UpdatePlayerInfo()
    {
        if (UserData != null && CurrentNode != null)
        {
            PlayerInfo = $"Captain {UserData.Username}\nCargo: {CargoHolds}/300\nRealm: {CurrentNode.RealmName}";
        }
    }

    [RelayCommand]
    private void ToggleNavigation()
    {
        ShowNavigation = !ShowNavigation;
    }



    [RelayCommand]
    private async Task NavigateToNodeAsync(ConnectedNodeDto targetNode)
    {
        if (IsBusy || UserData == null || CurrentNode == null) return;

        try
        {
            IsBusy = true;
            ClearError();

            var request = new MoveRequestDto
            {
                UserId = UserData.UserId,
                FromNodeId = CurrentNode.Id,
                ToNodeId = targetNode.NodeId
            };

            var response = await _apiService.MoveToNodeAsync(request);
            if (response != null && response.Success)
            {
                CurrentNode = response.CurrentNode;
                ShowNavigation = false;
                
                // Show arrival message
                await DebugHelper.SafeDisplayAlertAsync(
                    "Navigation Complete", 
                    response.Message, 
                    "OK");
            }
            else
            {
                SetError("Navigation failed");
            }
        }
        catch (Exception ex)
        {
            SetError($"Navigation error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RefreshNodeAsync()
    {
        if (IsBusy || CurrentNode == null) return;

        try
        {
            IsBusy = true;
            ClearError();

            var nodeDetails = await _apiService.GetNodeDetailsAsync(CurrentNode.Id);
            if (nodeDetails != null)
            {
                CurrentNode = nodeDetails;
            }
        }
        catch (Exception ex)
        {
            SetError($"Refresh error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task BackToRealmsAsync()
    {
        if (UserData != null)
        {
            await DebugHelper.SafeNavigateAsync("//realms", new Dictionary<string, object>
            {
                ["UserData"] = UserData
            });
        }
    }

    [RelayCommand]
    private async Task ShowPlanetsAsync()
    {
        if (CurrentNode == null || CurrentNode.PlanetCount == 0) return;

        // TODO: Implement planet selection popup
        await DebugHelper.SafeDisplayAlertAsync(
            "Planets", 
            $"This system has {CurrentNode.PlanetCount} planets. Planet selection interface coming soon!", 
            "OK");
    }

    [RelayCommand]
    private async Task ShowTradeAsync()
    {
        if (CurrentNode?.HasQuantumStation != true) return;

        // TODO: Implement trading interface
        await DebugHelper.SafeDisplayAlertAsync(
            "Quantum Station Trading", 
            "Trading interface coming soon! You can trade resources at this quantum station.", 
            "OK");
    }

    [RelayCommand]
    private async Task ShowTacticalAsync()
    {
        if (CurrentNode == null) return;

        // TODO: Implement tactical view
        await DebugHelper.SafeDisplayAlertAsync(
            "Tactical View", 
            "Tactical interface coming soon! View other players and threats in this system.", 
            "OK");
    }
}