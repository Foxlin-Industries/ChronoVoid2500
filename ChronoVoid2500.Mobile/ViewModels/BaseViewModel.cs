using CommunityToolkit.Mvvm.ComponentModel;

namespace ChronoVoid2500.Mobile.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public void SetError(string message)
    {
        ErrorMessage = message;
    }

    public void ClearError()
    {
        ErrorMessage = string.Empty;
    }
}