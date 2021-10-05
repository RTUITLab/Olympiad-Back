using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympiad.ControlPanel.JsWrappers
{
    public static class DownloadFileWrapper
    {
        public static async Task DownloadFile(this IJSRuntime jSRuntime, string fileName, string content)
        {
            await jSRuntime.DownloadFile(fileName, Encoding.UTF8.GetBytes(content));
        }
        public static async Task<string> DownloadFile(this IJSRuntime jSRuntime, string fileName, byte[] data)
        {
            return await jSRuntime.InvokeAsync<string>("saveAsFile", fileName, data);
        }
        public static async Task RevokeUrl(this IJSRuntime jSRuntime, string url)
        {
            await jSRuntime.InvokeVoidAsync("revokeURL", url);
        }
    }
}
