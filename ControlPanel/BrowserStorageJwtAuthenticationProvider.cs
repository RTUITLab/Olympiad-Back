using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Olympiad.ControlPanel.Services;
using PublicAPI.Responses;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Olympiad.ControlPanel;
public class BrowserStorageJwtAuthenticationProvider : AuthenticationStateProvider, ILoginRefresh
{
    public const string LOGIN_TYPE_CLAIM = nameof(BrowserStorageJwtAuthenticationProvider) + "_" + nameof(LOGIN_TYPE_CLAIM);
    public const string TEMP_LOGIN = nameof(TEMP_LOGIN);
    public const string PERMANENT_LOGIN = nameof(PERMANENT_LOGIN);

    private static readonly AuthenticationState EMPTY_STATE = new(new ClaimsPrincipal());
    private const string LOCAL_STORAGE_ACCESS_TOKEN_KEY = "userToken";

    private readonly ILocalStorageService localStorageService;
    private readonly ISessionStorageService sessionStorageService;
    private readonly AccessTokenProvider accessTokenProvider;
    private readonly HttpClient httpClient;

    public BrowserStorageJwtAuthenticationProvider(
        ILocalStorageService localStorageService,
        ISessionStorageService sessionStorageService,
        AccessTokenProvider accessTokenProvider,
        HttpClient httpClient)
    {
        this.localStorageService = localStorageService;
        this.sessionStorageService = sessionStorageService;
        this.accessTokenProvider = accessTokenProvider;
        this.httpClient = httpClient;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var loginType = TEMP_LOGIN;
        var userToken = await sessionStorageService.GetItemAsStringAsync(LOCAL_STORAGE_ACCESS_TOKEN_KEY);
        if (string.IsNullOrEmpty(userToken))
        {
            userToken = await localStorageService.GetItemAsStringAsync(LOCAL_STORAGE_ACCESS_TOKEN_KEY);
            loginType = PERMANENT_LOGIN;
            if (string.IsNullOrEmpty(userToken))
            {
                return EMPTY_STATE;
            }
        }

        var responseBody = await GetMe(userToken);
        return responseBody.Match(
            meInfo => HandleMeResult(meInfo, loginType),
            state => state);
    }

    private AuthenticationState HandleMeResult(GetMeResult meInfo, string loginType)
    {
        accessTokenProvider.AccessToken = meInfo.Token;
        //httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", meInfo.Token);

        var baseClaims = new[] {
                    new Claim(ClaimTypes.NameIdentifier, meInfo.Id.ToString()),
                    new Claim(ClaimTypes.Name, meInfo.FirstName),
                    new Claim(LOGIN_TYPE_CLAIM, loginType)
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
