using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using ChronoVoid2500.Mobile.Services;
using ChronoVoid2500.Mobile.ViewModels;
using ChronoVoid2500.Mobile.Views;
using ChronoVoid2500.Mobile.Converters;

namespace ChronoVoid2500.Mobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Register HTTP Client
		builder.Services.AddHttpClient<ApiService>(client =>
		{
			client.BaseAddress = new Uri("https://localhost:7001/");
			client.Timeout = TimeSpan.FromSeconds(30);
		});

		// Register Services
		builder.Services.AddSingleton<ApiService>();

		// Register ViewModels
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<RealmsViewModel>();
		builder.Services.AddTransient<GameViewModel>();

		// Register Views
		builder.Services.AddTransient<DebugPage>();
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<RealmsPage>();
		builder.Services.AddTransient<GamePage>();

		// Register Value Converters
		builder.Services.AddSingleton<StringToBoolConverter>();
		builder.Services.AddSingleton<InvertedBoolConverter>();
		builder.Services.AddSingleton<BoolToStringConverter>();
		builder.Services.AddSingleton<BoolToColorConverter>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
