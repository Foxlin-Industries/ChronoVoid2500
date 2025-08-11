using ChronoVoid2500.Mobile.ViewModels;

namespace ChronoVoid2500.Mobile.Views;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _viewModel;

    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        
        // Set up modern interface interactions
        SetupModernInterface();
    }

    private void SetupModernInterface()
    {
        // Set up Enter key handling for smooth UX
        UsernameEntry.Completed += (s, e) => 
        {
            if (_viewModel.IsRegistering)
                EmailEntry.Focus();
            else
                PasswordEntry.Focus();
        };

        EmailEntry.Completed += (s, e) => PasswordEntry.Focus();
        
        PasswordEntry.Completed += async (s, e) => 
        {
            if (_viewModel.IsRegistering)
                await _viewModel.RegisterCommand.ExecuteAsync(null);
            else
                await _viewModel.LoginCommand.ExecuteAsync(null);
        };

        // Set up password visibility toggle
        TogglePasswordButton.Clicked += (s, e) => 
        {
            _viewModel.IsPasswordHidden = !_viewModel.IsPasswordHidden;
            TogglePasswordButton.Text = _viewModel.IsPasswordHidden ? "👁️" : "🙈";
        };

        // Set up mode toggle buttons
        LoginModeButton.Clicked += (s, e) => 
        {
            if (_viewModel.IsRegistering)
            {
                _viewModel.IsRegistering = false;
                UpdateModeButtons();
            }
        };

        RegisterModeButton.Clicked += (s, e) => 
        {
            if (!_viewModel.IsRegistering)
            {
                _viewModel.IsRegistering = true;
                UpdateModeButtons();
            }
        };

        // Set up action button
        ActionButton.Clicked += async (s, e) => 
        {
            if (_viewModel.IsRegistering)
                await _viewModel.RegisterCommand.ExecuteAsync(null);
            else
                await _viewModel.LoginCommand.ExecuteAsync(null);
        };

        // Listen for property changes to update UI
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(LoginViewModel.IsRegistering))
            {
                UpdateModeButtons();
                UpdateActionButton();
            }
        };

        // Initial setup
        UpdateModeButtons();
        UpdateActionButton();
    }

    private void UpdateModeButtons()
    {
        if (_viewModel.IsRegistering)
        {
            LoginModeButton.BackgroundColor = Color.FromArgb("#333333");
            LoginModeButton.TextColor = Colors.White;
            RegisterModeButton.BackgroundColor = Color.FromArgb("#00ff88");
            RegisterModeButton.TextColor = Colors.Black;
        }
        else
        {
            LoginModeButton.BackgroundColor = Color.FromArgb("#00ff88");
            LoginModeButton.TextColor = Colors.Black;
            RegisterModeButton.BackgroundColor = Color.FromArgb("#333333");
            RegisterModeButton.TextColor = Colors.White;
        }
    }

    private void UpdateActionButton()
    {
        ActionButton.Text = _viewModel.IsRegistering ? "CREATE ACCOUNT" : "ENTER VOID";
    }

    private void SetupModernInterface()
    {
        // Set up Enter key handling
        UsernameEntry.Completed += (s, e) => 
        {
            if (_viewModel.IsRegistering)
                EmailEntry.Focus();
            else
                PasswordEntry.Focus();
        };

        EmailEntry.Completed += (s, e) => PasswordEntry.Focus();
        PasswordEntry.Completed += async (s, e) => 
        {
            if (_viewModel.IsRegistering)
                await _viewModel.RegisterCommand.ExecuteAsync(null);
            else
                await _viewModel.LoginCommand.ExecuteAsync(null);
        };

        // Set up password visibility toggle
        TogglePasswordButton.Clicked += (s, e) => 
        {
            _viewModel.IsPasswordHidden = !_viewModel.IsPasswordHidden;
            TogglePasswordButton.Text = _viewModel.IsPasswordHidden ? "👁️" : "🙈";
        };

        // Set up mode toggle buttons
        LoginModeButton.Clicked += (s, e) => 
        {
            if (_viewModel.IsRegistering)
            {
                _viewModel.IsRegistering = false;
                UpdateModeButtons();
            }
        };

        RegisterModeButton.Clicked += (s, e) => 
        {
            if (!_viewModel.IsRegistering)
            {
                _viewModel.IsRegistering = true;
                UpdateModeButtons();
            }
        };

        // Set up action button
        ActionButton.Clicked += async (s, e) => 
        {
            if (_viewModel.IsRegistering)
                await _viewModel.RegisterCommand.ExecuteAsync(null);
            else
                await _viewModel.LoginCommand.ExecuteAsync(null);
        };

        // Listen for property changes
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(LoginViewModel.IsRegistering))
            {
                UpdateModeButtons();
                UpdateActionButton();
            }
        };

        // Initial setup
        UpdateModeButtons();
        UpdateActionButton();
    }

    private void UpdateModeButtons()
    {
        if (_viewModel.IsRegistering)
        {
            LoginModeButton.BackgroundColor = Color.FromArgb("#333333");
            LoginModeButton.TextColor = Colors.White;
            RegisterModeButton.BackgroundColor = Color.FromArgb("#00ff88");
            RegisterModeButton.TextColor = Colors.Black;
        }
        else
        {
            LoginModeButton.BackgroundColor = Color.FromArgb("#00ff88");
            LoginModeButton.TextColor = Colors.Black;
            RegisterModeButton.BackgroundColor = Color.FromArgb("#333333");
            RegisterModeButton.TextColor = Colors.White;
        }
    }

    private void UpdateActionButton()
    {
        ActionButton.Text = _viewModel.IsRegistering ? "CREATE ACCOUNT" : "ENTER VOID";
    }
}