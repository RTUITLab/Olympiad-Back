using AntDesign;
using System.Threading.Tasks;

namespace Olympiad.ControlPanel.Extensions
{
    public static class MessageServiceExtensions
    {
        public static void ShowWarning(this MessageService messageService, string content)
        {
            Task.Run(() => messageService.Warning(content));
        }
        public static void ShowSuccess(this MessageService messageService, string content)
        {
            Task.Run(() => messageService.Success(content));
        }
    }
}
