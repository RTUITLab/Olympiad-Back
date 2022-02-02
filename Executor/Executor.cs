using Executor.Executers;
using Executor.Executers.Build;
using Executor.Executers.Run;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using Executor.Models.Settings;
using Microsoft.Extensions.Options;
using System.Xml.XPath;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Olympiad.Shared.Models.Settings;

namespace Executor
{
    class Executor
    {
        private ISolutionsBase solutionBase;
        private readonly IOptions<RabbitMqQueueSettings> rabbitMQOptinos;
        private readonly ILogger<Executor> logger;
        public readonly List<ExecuteWorker> executeWorkers;


        public Executor(
            ISolutionsBase solutionBase,
            IDockerClient dockerClient,
            ILoggerFactory loggerFactory,
            IOptions<RunningSettings> runningOptions,
            IOptions<RabbitMqQueueSettings> rabbitMQOptinos,
            IOptions<StartSettings> startOptions,
            ILogger<Executor> logger)
        {
            executeWorkers = Enumerable.Repeat(0, runningOptions.Value.WorkersCount)
                .Select(n =>
                            new ExecuteWorker(
                                    solutionBase.SaveChanges,
                                    solutionBase.SaveBuildLog,
                                    solutionBase.GetExerciseData,
                                    solutionBase,
                                    dockerClient,
                                    runningOptions.Value,
                                    startOptions.Value,
                                    loggerFactory)
                        )
                .ToList();
            this.solutionBase = solutionBase;
            this.rabbitMQOptinos = rabbitMQOptinos;
            this.logger = logger;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            try
            {
                SubscibeWorkers();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Can't subscribe workers to rabbitmq");
                throw new Exception("Can't subscribe workers to rabbitmq", ex);
            }
            await Task.Delay(-1);
        }

        private IConnection SubscibeWorkers()
        {
            var factory = new ConnectionFactory()
            {
                Uri = new Uri(rabbitMQOptinos.Value.Uri),
                ClientProvidedName = rabbitMQOptinos.Value.ClientProvidedName,
                DispatchConsumersAsync = true
            };
            var connection = factory.CreateConnection();
            foreach (var worker in executeWorkers)
            {
                CreateChannel(connection, out var channel, out var consumer);
                consumer.Received += async (model, ea) =>
                {
                    await HandleSolution(ea, worker, channel);
                };
                channel.BasicConsume(queue: rabbitMQOptinos.Value.QueueName, autoAck: false, consumer: consumer);
            }
            logger.LogInformation("Start listening");
            return connection;
        }

        private async Task HandleSolution(BasicDeliverEventArgs ea, ExecuteWorker worker, IModel channel)
        {
            var body = ea.Body.ToArray();
            Guid solutinoId;
            try
            {
                solutinoId = new Guid(body);
            }
            catch
            {
                logger.LogWarning("Incorrect data");
                return;
            }

            var solution = await solutionBase.GetSolutionInfo(solutinoId);
            await worker.Handle(solution);

            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }

        private void CreateChannel(IConnection connection, out IModel channel, out AsyncEventingBasicConsumer consumer)
        {
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: rabbitMQOptinos.Value.QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            consumer = new AsyncEventingBasicConsumer(channel);
        }
    }
}
