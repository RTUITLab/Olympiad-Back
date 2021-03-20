using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultsViewer.Extensions
{
    public static class JsExtensions
    {
        public static async Task DownloadFile(this IJSRuntime jSRuntime, string fileName, string content)
        {
            await jSRuntime.DownloadFile(fileName, Encoding.UTF8.GetBytes(content));
        }
        public static async Task DownloadFile(this IJSRuntime jSRuntime, string fileName, byte[] data)
        {
            await jSRuntime.InvokeVoidAsync("saveAsFile", fileName, data);
        }
    }
}
