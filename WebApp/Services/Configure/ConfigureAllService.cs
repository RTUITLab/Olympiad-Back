using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WebApp.Services.Configure
{
    public class ConfigureAllService : IHostedService
    {
        private readonly ServicesList servicesList;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<ConfigureAllService> logger;

        public ConfigureAllService(ServicesList servicesList, IServiceScopeFactory serviceScopeFactory, ILogger<ConfigureAllService> logger)
        {
            this.servicesList = servicesList;
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }
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
            var delay = TimeSpan.FromSeconds(5);;
            while (tryNum++ < 5)
            {
                try
                {
                    logger.LogInformation("Configuring {workType} try #{tryNum}", targetType.Name, tryNum);
                    using var scope = serviceScopeFactory.CreateScope();
                    if (!(scope.ServiceProvider.GetService(targetType) is IConfigureWork work))
                    {
                        throw new ArgumentException($"type must implements {nameof(IConfigureWork)}", nameof(targetType));
                    }
                    await work.Configure(cancellationToken);
                    return;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Error while configuring type {workType}, wait {delay} to retry");
                }
            }
            throw new Exception("Can't configre app on startup, please read logs. Crashing...");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        public class ServicesList
        {
            public List<Type> ServicesTypes { get; }

            public ServicesList(List<Type> servicesTypes)
            {
                ServicesTypes = servicesTypes;
            }
        }
    }
}
