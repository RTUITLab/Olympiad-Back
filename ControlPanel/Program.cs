using Olympiad.ControlPanel;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.SessionStorage;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Olympiad.ControlPanel.Services;
using Refit;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration.GetConnectionString("ApiBaseUrl")) });

builder.Services.AddAntDesign();

builder.Services.AddScoped<IControlPanelApiService>(sp => RestService.For<IControlPanelApiService>(sp.GetRequiredService<HttpClient>()));
builder.Services.AddScoped<ILoginRefresh>(sp => (LocalStorageJwtAuthenticationProvider)sp.GetRequiredService<AuthenticationStateProvider>());


builder.Services.AddScoped<AuthenticationStateProvider, LocalStorageJwtAuthenticationProvider>();
builder.Services.AddAuthorizationCore();




builder.Services.AddBlazoredSessionStorage();
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
