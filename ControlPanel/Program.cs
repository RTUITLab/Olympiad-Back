using Olympiad.ControlPanel;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.SessionStorage;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Olympiad.ControlPanel.Services;
using Refit;
using System.Globalization;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Olympiad.Shared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

Uri baseAddress;
if (builder.HostEnvironment.IsDevelopment())
{
    baseAddress = new Uri(builder.Configuration.GetConnectionString("ApiBaseUrl"));
} else
{
    var uriBuilder = new UriBuilder(builder.HostEnvironment.BaseAddress)
    {
        Path = ""
    };
    baseAddress = uriBuilder.Uri;
}

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = baseAddress });

builder.Services.AddAntDesign();



var refitSettings = new RefitSettings
{
    ContentSerializer = new SystemTextJsonContentSerializer(new System.Text.Json.JsonSerializerOptions
    {
        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    })
};
builder.Services.AddScoped<IControlPanelApiService>(sp => RestService.For<IControlPanelApiService>(sp.GetRequiredService<HttpClient>(), refitSettings));
builder.Services.AddScoped<IChallengesApi>(sp => RestService.For<IChallengesApi>(sp.GetRequiredService<HttpClient>(), refitSettings));
builder.Services.AddScoped<IRolesApi>(sp => RestService.For<IRolesApi>(sp.GetRequiredService<HttpClient>(), refitSettings));
builder.Services.AddScoped<ILoginRefresh>(sp => (LocalStorageJwtAuthenticationProvider)sp.GetRequiredService<AuthenticationStateProvider>());


builder.Services.AddScoped<AuthenticationStateProvider, LocalStorageJwtAuthenticationProvider>();
builder.Services.AddAuthorizationCore();

builder.Services.AddTransient<UserPasswordGenerator>();
builder.Services.AddScoped<GenerateUserService>();


builder.Services.AddBlazoredSessionStorage();
builder.Services.AddBlazoredLocalStorage();
var ruCulture = new CultureInfo("ru-RU", false);
CultureInfo.CurrentCulture = ruCulture;
CultureInfo.CurrentUICulture = ruCulture;
CultureInfo.DefaultThreadCurrentCulture = ruCulture;
CultureInfo.DefaultThreadCurrentUICulture = ruCulture;

await builder.Build().RunAsync();
