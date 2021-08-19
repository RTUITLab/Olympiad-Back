using Olympiad.ControlPanel;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.SessionStorage;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration.GetConnectionString("ApiBaseUrl")) });

builder.Services.AddScoped<ILoginRefresh>(sp => (LocalStorageJwtAuthenticationProvider)sp.GetRequiredService<AuthenticationStateProvider>());


builder.Services.AddScoped<AuthenticationStateProvider, LocalStorageJwtAuthenticationProvider>();
builder.Services.AddAuthorizationCore();




builder.Services.AddBlazoredSessionStorage();
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
