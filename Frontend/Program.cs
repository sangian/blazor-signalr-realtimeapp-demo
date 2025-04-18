using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Frontend;
using Frontend.Services;
using GoogleMapsComponents;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Text;
using Blazored.LocalStorage;
using Frontend.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Ljbc1994.Blazor.IntersectionObserver;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Load appsettings.json
using var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var appSettingsJson = await http.GetStringAsync("appsettings.json");
var configuration = new ConfigurationBuilder()
    .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettingsJson)))
    .Build();

// Configure Authentication
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();

// Configure HttpClient
var backendApiBaseUrl = configuration["BackendOptions:BaseUrl"];
builder.Services.AddTransient<CustomAuthorizationMessageHandler>();
builder.Services.AddHttpClient("BackendAPI", client =>
{
    client.BaseAddress = new Uri(backendApiBaseUrl!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

// Configure Google Maps
var apiKey = configuration["GoogleMapsOptions:ApiKey"];
builder.Services.AddBlazorGoogleMaps(apiKey!);

// Configure Fluent UI Components
builder.Services.AddFluentUIComponents();

// Configure other services
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddIntersectionObserver();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<FlightService>();
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<Frontend.Services.MessageService>();
builder.Services.AddScoped<ChatServerService>();
builder.Services.AddScoped<LiveTelemetryService>();

builder.Services.AddSingleton<RoomManager>();

await builder.Build().RunAsync();
