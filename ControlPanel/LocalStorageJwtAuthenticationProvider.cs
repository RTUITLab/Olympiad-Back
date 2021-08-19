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
    private static readonly AuthenticationState EMPTY_STATE = new(new ClaimsPrincipal());
    private const string LOCAL_STORAGE_ACCESS_TOKEN_KEY = "userToken";

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
        var userToken = await localStorageService.GetItemAsStringAsync(LOCAL_STORAGE_ACCESS_TOKEN_KEY);
        if (string.IsNullOrEmpty(userToken))
        {
            return EMPTY_STATE;
        }

        var responseBody = await GetMe(userToken);
        return await responseBody.Match(
            meInfo => HandleMeResult(meInfo),
            state => ValueTask.FromResult(state));
    }

	private async ValueTask<AuthenticationState> HandleMeResult(GetMeResult meInfo)
	{
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", meInfo.Token);
        await localStorageService.SetItemAsStringAsync(LOCAL_STORAGE_ACCESS_TOKEN_KEY, meInfo.Token);

        var baseClaims = new[] {
                    new Claim(ClaimTypes.NameIdentifier, meInfo.Id.ToString()),
                    new Claim(ClaimTypes.Name, meInfo.FirstName)
            };
        var baseClaimsIdentity = new ClaimsIdentity(baseClaims, "baseClaimsIdentity");

        var specificClaims = meInfo.Claims.SelectMany(kvp => kvp.Value.Select(v => new Claim(kvp.Key, v))).ToArray();
        var specificClaimsIdentity = new ClaimsIdentity(specificClaims, "specificClaimsIdentity");
        
        var principal = new ClaimsPrincipal(new ClaimsIdentity[] { baseClaimsIdentity, specificClaimsIdentity });
        return new AuthenticationState(principal);
    }

	private async Task<OneOf.OneOf<GetMeResult, AuthenticationState>> GetMe(string userToken)
    {
        try
        {
            var message = new HttpRequestMessage(HttpMethod.Get, "api/auth/getme");
            message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userToken);
            var result = await httpClient.SendAsync(message);

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return EMPTY_STATE;
            }
            var raw = await result.Content.ReadAsStringAsync();
            var responseBody = JsonSerializer.Deserialize<GetMeResult>(raw, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            if (responseBody != null)
            {
                return responseBody;
            }
            else
            {
                return EMPTY_STATE;
            }
        }
        catch
        {
            return EMPTY_STATE;
        }
    }

    public async Task SaveTokenAndRefreshAsync(string? accessToken)
    {
        await localStorageService.SetItemAsStringAsync(LOCAL_STORAGE_ACCESS_TOKEN_KEY, accessToken);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
