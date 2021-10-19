
using Microsoft.AspNetCore.Components;
using System.Net;

namespace Olympiad.ControlPanel.Shared;
public class RedirectToLogin : ComponentBase
{
    [Inject]
    protected NavigationManager NavigationManager { get; set; }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (firstRender)
        {
            NavigationManager.NavigateTo($"login?returnTo={WebUtility.UrlEncode(NavigationManager.ToBaseRelativePath(NavigationManager.Uri))}");
        }
    }
}
