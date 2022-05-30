using AntDesign;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Olympiad.ControlPanel.Extensions
{
    public static class NotificationServiceExtensions
    {
        public static void ShowWarning(this NotificationService notificationService, string content)
            => Show(notificationService, content, NotificationType.Warning);
        public static void ShowSuccess(this NotificationService notificationService, string content)
            => Show(notificationService, content, NotificationType.Success);

        public static void ShowError(this NotificationService notificationService, string content)
            => Show(notificationService, content, NotificationType.Error);
        public static void ShowError(this NotificationService notificationService, NotificationConfig notificationConfig)
            => Task.Run(() => notificationService.Error(notificationConfig));


        public static void Show(this NotificationService notificationService, string content, NotificationType type)
        {
            Task.Run(() => notificationService.Open(new NotificationConfig
            {
                Message = content,
                NotificationType = type
            }));
        }
        public static void Show(this NotificationService notificationService, NotificationConfig notificationConfig)
        {
            Task.Run(() => notificationService.Open(notificationConfig));
        }
    }
}
