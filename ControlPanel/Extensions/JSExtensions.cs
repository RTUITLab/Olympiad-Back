
using Microsoft.JSInterop;

namespace Olympiad.ControlPanel.Extensions;
public static class JSExtensions
{
    public static ValueTask Log(this IJSRuntime jsRuntime, params object?[]? data)
    {
        return jsRuntime.InvokeVoidAsync("console.log", data);
    }
    public static ValueTask OpenLinkInNewTab(this IJSRuntime jsRuntime, string url)
    {
        return jsRuntime.InvokeVoidAsync("openLinkInNewTab", url);
    }
}
