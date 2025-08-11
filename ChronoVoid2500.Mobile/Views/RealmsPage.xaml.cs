using ChronoVoid2500.Mobile.ViewModels;

namespace ChronoVoid2500.Mobile.Views;

public partial class RealmsPage : ContentPage
{
    private readonly RealmsViewModel _viewModel;

    public RealmsPage(RealmsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}