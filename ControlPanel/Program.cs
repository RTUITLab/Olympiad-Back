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
using Microsoft.AspNetCore.Components.Web;
using DiffPlex.DiffBuilder;
using DiffPlex;
using System.Threading.Tasks;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<ISideBySideDiffBuilder, SideBySideDiffBuilder>();
builder.Services.AddScoped<IDiffer, Differ>();

Uri baseAddress;
if (builder.HostEnvironment.IsDevelopment())
{
    baseAddress = new Uri(builder.Configuration.GetConnectionString("ApiBaseUrl"));
}
else
{
    var uriBuilder = new UriBuilder(builder.HostEnvironment.BaseAddress)
    {
        Path = ""
    };
    baseAddress = uriBuilder.Uri;
}

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = baseAddress });
builder.Services.AddSingleton(new AttachmentLinkGenerator(baseAddress));

builder.Services.AddAntDesign();


RegisterApiServices(builder);
builder.Services.AddScoped<ILoginRefresh>(sp => (BrowserStorageJwtAuthenticationProvider)sp.GetRequiredService<AuthenticationStateProvider>());



builder.Services.AddScoped<AuthenticationStateProvider, BrowserStorageJwtAuthenticationProvider>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AccessTokenProvider>();

builder.Services.AddTransient<UserPasswordGenerator>();
builder.Services.AddScoped<GenerateUserService>();
builder.Services.AddScoped<ChallengeTotalReportCreator>();


builder.Services.AddBlazoredSessionStorage();
builder.Services.AddBlazoredLocalStorage();
var ruCulture = new CultureInfo("ru-RU", false);
CultureInfo.CurrentCulture = ruCulture;
CultureInfo.CurrentUICulture = ruCulture;
CultureInfo.DefaultThreadCurrentCulture = ruCulture;
CultureInfo.DefaultThreadCurrentUICulture = ruCulture;

await builder.Build().RunAsync();


void RegisterApiServices(WebAssemblyHostBuilder builder)
{
    var jsonSerializer = new SystemTextJsonContentSerializer(new System.Text.Json.JsonSerializerOptions
    {
        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    });
    void RegisterApiService<T>() where T : class
    {
        builder.Services.AddScoped<T>(sp => RestService.For<T>(baseAddress.ToString(), new RefitSettings
        {
            ContentSerializer = jsonSerializer,
            AuthorizationHeaderValueGetter = () => Task.FromResult(sp.GetRequiredService<AccessTokenProvider>().AccessToken ?? "")
        }));
    }

    RegisterApiService<IControlPanelApiService>();

    RegisterApiService<IChallengesApi>();
    RegisterApiService<IExercisesApi>();
    RegisterApiService<ISolutionsApi>();
    RegisterApiService<IReportsApi>();

    RegisterApiService<IRolesApi>();
}