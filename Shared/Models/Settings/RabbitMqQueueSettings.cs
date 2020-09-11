using System.ComponentModel.DataAnnotations;

namespace Olympiad.Shared.Models.Settings
{
    public class RabbitMqQueueSettings
    {
        [Required]
        public string Host { get; set; }
        [Required]
        public string QueueName { get; set; }
    }
}
