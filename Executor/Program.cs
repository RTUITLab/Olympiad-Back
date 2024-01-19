using System;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Executor.Models.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Olympiad.Shared.Models.Settings;

namespace Executor;

class Program
{
    private static IConfiguration configuration;

    static async Task Main(string[] args)
    {
        Console.WriteLine($"Started {DateTimeOffset.Now}");
        try
        {

            configuration = SetupConfigs(args);

            var servicesProvider = BuildServices();
            var versionConfig = servicesProvider.GetRequiredService<IOptions<StartSettings>>();

            Console.WriteLine($"Version: {versionConfig.Value.VersionString}");

            if (!await IsDockerAvailable(servicesProvider.GetRequiredService<IDockerClient>()))
            {
                throw new Exception("host must see docker!");
            }
            var executor = servicesProvider.GetRequiredService<Executor>();
            var statusReporter = servicesProvider.GetRequiredService<ConsoleStatusReporter>();
            var statusReporterTask = configuration.GetConsoleMode() == ConsoleMode.StatusReporting ?
                statusReporter.Start(executor, CancellationToken.None)
                :
                Task.CompletedTask;

            var dockerImagesDownloader = servicesProvider.GetRequiredService<DockerImagesDownloader>();
            await dockerImagesDownloader.DownloadBaseImages();

            await Task.WhenAll(
                executor.Start(CancellationToken.None),
                statusReporterTask);
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            Console.Error.WriteLine(ex.StackTrace);
            Console.Error.WriteLine($"Exited {DateTime.UtcNow}");
            Environment.Exit(1);
        }
    }

    private static async Task<bool> IsDockerAvailable(IDockerClient dockerClient)
    {
        try
        {
            await dockerClient.System.PingAsync();
            return true;
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync(ex.Message);
            return false;
        }
    }

    private static IServiceProvider BuildServices()
        => new ServiceCollection()
            .AddLogging(configure =>
            {
                if (configuration.GetConsoleMode() == ConsoleMode.Logs)
                {
                    configure.AddConsole();
                }
                configure.AddProvider(new ConsoleStatusReporterLoggerProvider());
                configure.AddConfiguration(configuration.GetSection("Logging"));
            })
            .Configure<StartSettings>(configuration.GetSection(nameof(StartSettings)))
            .Configure<UserInfo>(configuration.GetSection(nameof(UserInfo)))
            .ConfigureAndValidate<RabbitMqQueueSettings>(configuration.GetSection(nameof(RabbitMqQueueSettings)))
            .ConfigureAndValidate<RunningSettings>(configuration.GetSection(nameof(RunningSettings)))
            .AddTransient<ISolutionsBase, DbManager>()
            .AddSingleton<Executor>()
            .AddSingleton<ConsoleStatusReporter>()
            .AddHttpClient(DbManager.DbManagerHttpClientName, (sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<StartSettings>>();
                client.BaseAddress = new Uri(options.Value.Address);
                client.DefaultRequestHeaders.Add("Executor-Version", options.Value.VersionString);
            })
            .Services
            .AddSingleton<IDockerClient>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<StartSettings>>();
                DockerClientConfiguration dockerConfig;
                if (string.IsNullOrEmpty(options.Value.DockerEndPoint))
                {
                    // take local docker
                    dockerConfig = new DockerClientConfiguration();
                }
                else
                {
                    dockerConfig = new DockerClientConfiguration(new Uri(options.Value.DockerEndPoint));
                }
                return dockerConfig.CreateClient();
            })
            .AddSingleton<DockerImagesDownloader>()
            .BuildServiceProvider();

    private static IConfiguration SetupConfigs(string[] args)
        => new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddJsonFile("appsettings.Local.json", optional: true)
            .AddEnvironmentVariables()
            .AddUserSecrets("b3bc0ade-140e-45d8-aa42-b4c85ef7fe4b") // TODO: rewrite executor to worker template
            .Build();
}
