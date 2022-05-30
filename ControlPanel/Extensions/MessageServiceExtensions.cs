using AntDesign;
using System.Threading.Tasks;

namespace Olympiad.ControlPanel.Extensions
{
    public static class MessageServiceExtensions
    {
        public static void ShowWarning(this MessageService messageService, string content)
            => Show(messageService, content, MessageType.Warning);
        public static void ShowSuccess(this MessageService messageService, string content)
            => Show(messageService, content, MessageType.Success);
        public static void Show(this MessageService messageService, string content, MessageType type)
        {
            Task.Run(() => messageService.Open(new MessageConfig
            {
                Content = content,
                Type = type
            }));
        }
    }
}
