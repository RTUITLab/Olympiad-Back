using System.ComponentModel.DataAnnotations;

namespace Olympiad.Shared.Models.Settings
{
    public class RabbitMqQueueSettings
    {
        [Required]
        public string ClientProvidedName { get; set; }
        [Required]
        public string QueueName { get; set; }
        /// <summary>
        /// Rabbit MQ uri <see href="https://www.rabbitmq.com/dotnet-api-guide.html#connecting"/>
        /// </summary>
        [Required]
        public string Uri { get; set; }
    }
}
