﻿using System.ComponentModel.DataAnnotations;

namespace Olympiad.Shared.Models.Settings
{
    public class RabbitMqQueueSettings
    {
        [Required]
        public string Host { get; set; }
        [Required]
        public string ClientProvidedName { get; set; }
        [Required]
        public string QueueName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string UserName { get; set; }
    }
}
