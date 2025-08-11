using ChronoVoid2500.Mobile.Views;

namespace ChronoVoid2500.Mobile;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		
		// Register routes for navigation
		Routing.RegisterRoute("login", typeof(LoginPage));
		Routing.RegisterRoute("realms", typeof(RealmsPage));
		Routing.RegisterRoute("game", typeof(GamePage));
	}
}
