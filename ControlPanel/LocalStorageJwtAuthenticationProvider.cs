using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using PublicAPI.Responses;
using System.Security.Claims;
using System.Text.Json;

namespace Olympiad.ControlPanel;
public class LocalStorageJwtAuthenticationProvider : AuthenticationStateProvider, ILoginRefresh
{
    private readonly ILocalStorageService localStorageService;
    private readonly IJSRuntime js;
    private readonly HttpClient httpClient;

    public LocalStorageJwtAuthenticationProvider(
        ILocalStorageService localStorageService,
        IJSRuntime JS,
        HttpClient httpClient)
    {
        this.localStorageService = localStorageService;
        js = JS;
        this.httpClient = httpClient;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var userToken = await localStorageService.GetItemAsync<string>("userToken");
        if (string.IsNullOrEmpty(userToken))
        {
            return new AuthenticationState(new ClaimsPrincipal());
        }

        var message = new HttpRequestMessage(HttpMethod.Get, "api/auth/getme");
        message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userToken);
        var result = await httpClient.SendAsync(message);

        if (result.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return new AuthenticationState(new ClaimsPrincipal());
        }
        var raw = await result.Content.ReadAsStringAsync();
        var responseBody = JsonSerializer.Deserialize<GetMeResult>(raw, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }) 
            ?? throw new ArgumentException("Incorrect json response");

        var baseClaims = new[] {
                    new Claim(ClaimTypes.NameIdentifier, responseBody.Id.ToString()),
                    new Claim(ClaimTypes.Name, responseBody.FirstName)
            };
        var baseClaimsIdentity = new ClaimsIdentity(baseClaims, "baseClaimsIdentity");
        var specificClaims = responseBody.Claims.SelectMany(kvp => kvp.Value.Select(v => new Claim(kvp.Key, v))).ToArray();
        var specificClaimsIdentity = new ClaimsIdentity(specificClaims, "specificClaimsIdentity");
        var principal = new ClaimsPrincipal(new ClaimsIdentity[] { baseClaimsIdentity, specificClaimsIdentity });
        return new AuthenticationState(principal);
    }

    public async Task SaveTokenAndRefreshAsync(string? accessToken)
    {
        await localStorageService.SetItemAsStringAsync("userToken", accessToken);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
