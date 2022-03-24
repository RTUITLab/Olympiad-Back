using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Olympiad.ControlPanel.JsWrappers
{
    public static class WorkWithElementsWrapper
    {
        public static async ValueTask ScrollToElement(this IJSRuntime js, ElementReference? element)
        {
            if (element is null)
            {
                return;
            }
            await js.InvokeAsync<string>("scrollToSpecificElement", element);
        }
        public static async ValueTask<string> GetInnerText(this IJSRuntime js, ElementReference? element)
        {
            if (element is null)
            {
                return "";
            }
            return await js.InvokeAsync<string>("getInnerTextOfElement", element);
        }
        public static ValueTask Click(this IJSRuntime js, ElementReference? element)
        {
            if (element is null)
            {
                return ValueTask.CompletedTask;
            }
            return js.InvokeVoidAsync("clickElement", element);
        }
    }
}
