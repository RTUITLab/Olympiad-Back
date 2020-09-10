using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Olympiad.Shared.Models.Settings
{
    public class RabbitMqQueueSettings
    {
        public string Host { get; set; }
        public string QueueName { get; set; }
    }
}
