using ChronoVoid2500.Mobile.Debug;

namespace ChronoVoid2500.Mobile;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		System.Diagnostics.Debug.WriteLine("App constructor called");
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		System.Diagnostics.Debug.WriteLine("CreateWindow called");
		
		var shell = new AppShell();
		var window = new Window(shell);
		
		// Add event handlers to track shell lifecycle
		shell.Loaded += (s, e) => 
		{
			System.Diagnostics.Debug.WriteLine("Shell Loaded event fired");
			DebugHelper.LogShellState("Shell.Loaded");
		};
		
		shell.Appearing += (s, e) => 
		{
			System.Diagnostics.Debug.WriteLine("Shell Appearing event fired");
			DebugHelper.LogShellState("Shell.Appearing");
		};
		
		window.Created += (s, e) => 
		{
			System.Diagnostics.Debug.WriteLine("Window Created event fired");
			DebugHelper.LogShellState("Window.Created");
		};
		
		System.Diagnostics.Debug.WriteLine("Returning window with shell");
		return window;
	}
}