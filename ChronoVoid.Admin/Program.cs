using Microsoft.EntityFrameworkCore;
using ChronoVoid.API.Data;
using ChronoVoid.API.Services;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add Radzen
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

// Add Entity Framework
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    "Host=localhost;Database=chronovoid2500;Username=postgres;Password=postgres";

builder.Services.AddDbContext<ChronoVoidContext>(options =>
    options.UseNpgsql(connectionString));

// Add services
builder.Services.AddScoped<AlienRaceGeneratorService>();
builder.Services.AddScoped<RealmGenerationService>();
builder.Services.AddScoped<PlanetService>();

// Add HTTP client for API calls
builder.Services.AddHttpClient("ChronoVoidAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:7000/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();