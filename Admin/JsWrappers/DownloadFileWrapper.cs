using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Olympiad.Admin.JsWrappers
{
    public static class DownloadFileWrapper
    {
        public static async Task DownloadFile(this IJSRuntime jSRuntime, string fileName, byte[] data)
        {
            await jSRuntime.InvokeVoidAsync("saveAsFile", fileName, data);
        }
    }
}
