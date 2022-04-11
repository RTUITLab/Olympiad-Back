using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Olympiad.ControlPanel.JsWrappers
{
    public class JsZipWrapper
    {
        public static async Task<JsZipWrapper> CreateInstance(IJSRuntime js)
        {
            var module = await js.InvokeAsync<IJSObjectReference>("import", "./js/jsZipWrapper.js");
            var jsWrapper = await  module.InvokeAsync<IJSObjectReference>("createZipArchive");
            return new JsZipWrapper(jsWrapper);
        }
        private IJSObjectReference jsWrapper;

        private JsZipWrapper(IJSObjectReference jsWrapper)
        {
            this.jsWrapper = jsWrapper;
        }
        public async ValueTask WriteText(string filePath, string content)
        {
            await jsWrapper.InvokeVoidAsync("textFile", filePath, content);
        }

        public async ValueTask WriteBinaryFromLink(string filePath, string link)
        {
            await jsWrapper.InvokeVoidAsync("blobFileFromLink", filePath, link);
        }

        public async ValueTask Download(string fileName)
        {
            await jsWrapper.InvokeVoidAsync("saveFile", fileName);
        }
    }
}
