using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WebApp.Services.Configure
{
    public class ConfigureAllService(
        ConfigureAllService.ServicesList servicesList,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<ConfigureAllService> logger) : IHostedService
    {
        private readonly ServicesList servicesList = servicesList;
        private readonly IServiceScopeFactory serviceScopeFactory = serviceScopeFactory;
        private readonly ILogger<ConfigureAllService> logger = logger;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Start configuring app");
            var serviceTypes = servicesList.ServicesTypes;
            foreach (var serviceType in serviceTypes)
            {
                await Configure(serviceType, cancellationToken);
            }
        }

        private async Task Configure(Type targetType, CancellationToken cancellationToken)
        {
            var tryNum = 0;
            var delay = TimeSpan.FromSeconds(5); ;
            while (tryNum++ < 5)
            {
                try
                {
                    logger.LogInformation("Configuring {WorkType} try #{TryNum}", targetType.Name, tryNum);
                    using var scope = serviceScopeFactory.CreateScope();
                    if (scope.ServiceProvider.GetService(targetType) is not IConfigureWork work)
                    {
                        throw new ArgumentException($"type must implements {nameof(IConfigureWork)}", nameof(targetType));
                    }
                    await work.Configure(cancellationToken);
                    return;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Error while configuring type {WorkType}, wait {Delay} to retry", targetType.Name, delay);
                }
            }
            throw new Exception("Can't configre app on startup, please read logs. Crashing...");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        public class ServicesList(List<Type> servicesTypes)
        {
            public List<Type> ServicesTypes { get; } = servicesTypes;
        }
    }
}
