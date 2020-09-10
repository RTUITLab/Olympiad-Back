using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Olympiad.Shared.Models.Settings;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Services.Interfaces;

namespace WebApp.Services
{
    public class RabbitMQQueue : IQueueChecker, IDisposable
    {
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly RabbitMqQueueSettings options;
        private readonly ILogger<RabbitMQQueue> logger;

        public RabbitMQQueue(
            IOptions<RabbitMqQueueSettings> options,
            ILogger<RabbitMQQueue> logger)
        {
            var factory = new ConnectionFactory() { HostName = options.Value.Host };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            this.options = options.Value;
            this.logger = logger;
        }
        public void Clear()
        {
            throw new NotImplementedException();
        }

        public List<Guid> GetFromQueue(int count)
        {
            throw new NotImplementedException();
        }

        public void PutInQueue(Guid solutionId)
        {
            var body = solutionId.ToByteArray();

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "",
                                 routingKey: options.QueueName,
                                 basicProperties: properties,
                                 body: body);

            logger.LogInformation($"Enqueue {solutionId}");
        }

        public void Dispose()
        {
            channel.Dispose();
            connection.Dispose();
        }
    }
}
